using CheckoutPaymentAPI.Models.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using CheckoutPaymentAPI.Tests.Core;
using System.Text;
using Moq;
using CheckoutPaymentAPI.Application.AcquiringBank;
using CheckoutPaymentAPI.Models.AcquiringBank;
using CheckoutPaymentAPI.Models;
using System.Linq;

namespace CheckoutPaymentAPI.IntegrationTests.Controllers
{
    [TestClass]
    [TestCategory("API - Integration - PaymentsController")]
    public class ProcessPaymentTests
    {
        [TestMethod]
        public async Task Returns_200_With_Success_Data_For_Successful_Payment()
        {
            const int RETURNED_PAYMENT_ID = 1;

            const decimal AMOUNT = .1m;
            const string INVALID_CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";

            var testNow = new DateTime(2021, 01, 01);
            var EXPIRY = new MonthYear { Year = testNow.AddYears(1).Year, Month = testNow.Month };

            var acqBankMock = new Mock<IAcquiringBank>();

            var (_, client, context) = Setup.CreateServer(new Setup.CreateServerOptions
            {
                TestNow = testNow,
                AcquiringBank = acqBankMock.Object
            });

            using (context)
            {
                acqBankMock.Setup(mock => mock.SendPayment(It.IsAny<AcquiringBankRequest>()))
                    .ReturnsAsync(new AcquiringBankResponse
                    {
                        Status = AcquiringBankResponseStatus.Success,
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
                var response = await client.PostAsync("/payments", requestContent);
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
            var EXPIRY = new MonthYear { Year = testNow.AddYears(1).Year, Month = testNow.Month };

            var acqBankMock = new Mock<IAcquiringBank>();

            var (_, client, context) = Setup.CreateServer(new Setup.CreateServerOptions
            {
                TestNow = testNow,
                AcquiringBank = acqBankMock.Object
            });

            using (context)
            {
                acqBankMock.Setup(mock => mock.SendPayment(It.IsAny<AcquiringBankRequest>()))
                    .ReturnsAsync(new AcquiringBankResponse
                    {
                        Status = AcquiringBankResponseStatus.Payment_Declined,
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
                var response = await client.PostAsync("/payments", requestContent);
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
            var EXPIRY = new MonthYear { Year = testNow.AddYears(1).Year, Month = testNow.Month };

            var (_, client, context) = Setup.CreateServer(new Setup.CreateServerOptions
            {
                TestNow = testNow
            });

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
                var response = await client.PostAsync("/payments", requestContent);
                Assert.AreEqual(400, (int)response.StatusCode);

                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ErrorResponseDTO>(responseContent);

                Assert.AreEqual("Validation error", data.Message);
                Assert.AreEqual(1, data.Errors.Count());
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
            var EXPIRY = new MonthYear { Year = testNow.AddYears(1).Year, Month = testNow.Month };

            var acqBankMock = new Mock<IAcquiringBank>();

            var (_, client, context) = Setup.CreateServer(new Setup.CreateServerOptions
            {
                TestNow = testNow
            });

            using (context)
            {
                acqBankMock.SetupSequence(mock => mock.SendPayment(It.IsAny<AcquiringBankRequest>()))
                    .ReturnsAsync(new AcquiringBankResponse
                    {
                        Status = AcquiringBankResponseStatus.Success,
                        PaymentId = FIRST_RETURNED_PAYMENT_ID
                    })
                    .ReturnsAsync(new AcquiringBankResponse
                    {
                        Status = AcquiringBankResponseStatus.Success,
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

                var response1 = await client.PostAsync("/payments", requestContent);
                response1.EnsureSuccessStatusCode();

                var response2 = await client.PostAsync("/payments", requestContent);
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
            var EXPIRY = new MonthYear { Year = testNow.AddYears(1).Year, Month = testNow.Month };

            var (_, client, context) = Setup.CreateServer(new Setup.CreateServerOptions
            {
                TestNow = testNow
            });

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

                var response = await client.PostAsync("/payments", requestContent);
                Assert.AreEqual(401, (int)response.StatusCode);
            }
        }
    }
}
