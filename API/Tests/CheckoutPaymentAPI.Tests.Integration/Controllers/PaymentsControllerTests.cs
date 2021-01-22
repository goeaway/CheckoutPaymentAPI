using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CheckoutPaymentAPI.Tests.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using CheckoutPaymentAPI.Core.Abstractions;
using CheckoutPaymentAPI.Core.Providers;
using Moq;
using CheckoutPaymentAPI.Core.Models;

namespace CheckoutPaymentAPI.IntegrationTests.Controllers
{
    [TestClass]
    [TestCategory("API - Integration - PaymentsController")]
    public class PaymentsControllerTests
    {
        private (TestServer server, HttpClient client, CheckoutPaymentAPIContext context, Mock<IAcquiringBank> mockBank) SetupServer(INowProvider nowProvider = null)
        {
            var context = Setup.CreateContext();
            var mockBank = new Mock<IAcquiringBank>();

            var server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton(context);

                    if(nowProvider != null)
                    {
                        services.AddSingleton(nowProvider);
                    }

                    services.AddSingleton(mockBank.Object);

                })
            );
            var client = server.CreateClient();

            return (server, client, context, mockBank);
        }

        [TestMethod]
        public async Task Returns_200_With_Success_Data_For_Successful_Payment()
        {
            const int RETURNED_PAYMENT_ID = 1;

            const decimal AMOUNT = .1m;
            const string INVALID_CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";

            var testNow = new DateTime(2021, 01, 01);
            var EXPIRY = testNow.AddYears(1);

            var nowProvider = new NowProvider(testNow);
            var (_, client, context, acqBankMock) = SetupServer(nowProvider);

            using (context)
            {
                acqBankMock.Setup(mock => mock.SendPayment())
                    .ReturnsAsync(new AcquiringBankResponse
                    {
                        Success = true,
                        PaymentId = RETURNED_PAYMENT_ID
                    });
                
                var request = new ProcessPaymentsRequestDTO
                {
                    Amount = AMOUNT,
                    CardNumber = INVALID_CARD_NUMBER,
                    CVV = CVV,
                    Expiry = EXPIRY,
                    Currency = CURRENCY,
                };

                var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("X-API-KEY", "CheckoutPaymentAPI-Q2hlY2tvdXRQYXltZW50QVBJ");
                var response = await client.PostAsync($"/payments/process", requestContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ProcessPaymentResponseDTO>(responseContent);

                Assert.AreEqual(RETURNED_PAYMENT_ID, data.PaymentId);
                Assert.IsTrue(data.Success);
            }
        }

        [TestMethod]
        public async Task Returns_200_With_Fail_Data_For_Failed_Payment()
        {
            const int RETURNED_PAYMENT_ID = 1;

            const decimal AMOUNT = .1m;
            const string INVALID_CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";

            var testNow = new DateTime(2021, 01, 01);
            var EXPIRY = testNow.AddYears(1);

            var nowProvider = new NowProvider(testNow);
            var (_, client, context, acqBankMock) = SetupServer(nowProvider);

            using (context)
            {
                acqBankMock.Setup(mock => mock.SendPayment())
                    .ReturnsAsync(new AcquiringBankResponse
                    {
                        Success = false,
                        PaymentId = RETURNED_PAYMENT_ID
                    });

                var request = new ProcessPaymentsRequestDTO
                {
                    Amount = AMOUNT,
                    CardNumber = INVALID_CARD_NUMBER,
                    CVV = CVV,
                    Expiry = EXPIRY,
                    Currency = CURRENCY,
                };

                var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("X-API-KEY", "CheckoutPaymentAPI-Q2hlY2tvdXRQYXltZW50QVBJ");
                var response = await client.PostAsync($"/payments/process", requestContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ProcessPaymentResponseDTO>(responseContent);

                Assert.AreEqual(RETURNED_PAYMENT_ID, data.PaymentId);
                Assert.IsFalse(data.Success);
            }
        }

        [TestMethod]
        public async Task Returns_400_For_Failed_Validation()
        {
            const decimal AMOUNT = .1m;
            const string INVALID_CARD_NUMBER = "4111111111111112";
            const string CVV = "123";
            const string CURRENCY = "GBP";

            var testNow = new DateTime(2021, 01, 01);
            var EXPIRY = testNow.AddYears(1);

            var nowProvider = new NowProvider(testNow);
            var (_, client, context, _) = SetupServer(nowProvider);

            using (context)
            {
                var request = new ProcessPaymentsRequestDTO
                {
                    Amount = AMOUNT,
                    CardNumber = INVALID_CARD_NUMBER,
                    CVV = CVV,
                    Expiry = EXPIRY,
                    Currency = CURRENCY,
                };

                var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("X-API-KEY", "CheckoutPaymentAPI-Q2hlY2tvdXRQYXltZW50QVBJ");
                var response = await client.PostAsync($"/payments/process", requestContent);
                Assert.AreEqual(400, (int)response.StatusCode);

                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ErrorResponseDTO>(responseContent);

                Assert.AreEqual("Validation error", data.Message);
                Assert.AreEqual(1, data.Errors.Count);
            }
        }

        [TestMethod]
        public async Task Returns_429_When_Same_Requests_Are_Blocked_If_Inside_TTL()
        {
            // api should cache a hash of the data in each process request and block any that are the same as currently live cache records
            const int FIRST_RETURNED_PAYMENT_ID = 1;
            const int SECOND_RETURNED_PAYMENT_ID = 2;

            const decimal AMOUNT = .1m;
            const string INVALID_CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";

            var testNow = new DateTime(2022, 01, 01);
            var EXPIRY = testNow.AddYears(1);

            var nowProvider = new NowProvider();
            var (_, client, context, acqBankMock) = SetupServer(nowProvider);

            using (context)
            {
                acqBankMock.SetupSequence(mock => mock.SendPayment())
                    .ReturnsAsync(new AcquiringBankResponse
                    {
                        Success = true,
                        PaymentId = FIRST_RETURNED_PAYMENT_ID
                    })
                    .ReturnsAsync(new AcquiringBankResponse
                    {
                        Success = true,
                        PaymentId = SECOND_RETURNED_PAYMENT_ID
                    });

                var request = new ProcessPaymentsRequestDTO
                {
                    Amount = AMOUNT,
                    CardNumber = INVALID_CARD_NUMBER,
                    CVV = CVV,
                    Expiry = EXPIRY,
                    Currency = CURRENCY,
                };

                client.DefaultRequestHeaders.Add("X-API-KEY", "CheckoutPaymentAPI-Q2hlY2tvdXRQYXltZW50QVBJ");
                var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                var response1 = await client.PostAsync("/payments/process", requestContent);
                response1.EnsureSuccessStatusCode();

                var response2 = await client.PostAsync("/payments/process", requestContent);
                Assert.AreEqual(429, (int)response2.StatusCode);
            }
        }

        [TestMethod]
        public async Task Returns_401_For_UnAuthed_Requests()
        {
            const decimal AMOUNT = .1m;
            const string INVALID_CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";

            var testNow = new DateTime(2022, 01, 01);
            var EXPIRY = testNow.AddYears(1);

            var nowProvider = new NowProvider();
            var (_, client, context, _) = SetupServer(nowProvider);

            using (context)
            {
                var request = new ProcessPaymentsRequestDTO
                {
                    Amount = AMOUNT,
                    CardNumber = INVALID_CARD_NUMBER,
                    CVV = CVV,
                    Expiry = EXPIRY,
                    Currency = CURRENCY,
                };
                client.DefaultRequestHeaders.Add("X-API-KEY", "CheckoutPaymentAPI-WrongAPIKey");

                var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/payments/process", requestContent);
                Assert.AreEqual(401, (int)response.StatusCode);
            }
        }
    }
}
