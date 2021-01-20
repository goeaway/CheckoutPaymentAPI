using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CheckoutPaymentAPI.Tests.Core;
using CheckoutPaymentAPI.Persistence.Models;

namespace CheckoutPaymentAPI.IntegrationTests.Controllers
{
    [TestClass]
    [TestCategory("Integration - PaymentDetailsController")]
    public class PaymentDetailsControllerTests
    {
        private (TestServer server, HttpClient client, CheckoutPaymentAPIContext context) SetupServer()
        {
            var context = Setup.CreateContext();

            var server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    services.AddSingleton(context);
                }));
            var client = server.CreateClient();

            return (server, client, context);
        }

        [TestMethod]
        public async Task Return_404_For_No_Id()
        {
            var (_, client, _) = SetupServer();

            var response = await client.GetAsync("/paymentdetails");

            Assert.AreEqual(404, (int)response.StatusCode);
        }

        [TestMethod]
        public async Task Return_200_With_Data_For_Successful_Find_And_Payment()
        {
            const int PAYMENT_ID = 1;
            const decimal AMOUNT = .1m;
            const string CARD_NUMBER = "123123423543";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            const bool PAYMENT_RESULT = true;
            var EXPIRY = new DateTime(2021, 01, 01);

            var (_, client, context) = SetupServer();

            using (context)
            {
                // add to DB so we can get it back
                context.ProcessedPayments.Add(new ProcessedPayment
                {
                    Id = PAYMENT_ID,
                    Amount = AMOUNT,
                    CardNumber = CARD_NUMBER,
                    CVV = CVV,
                    Expiry = EXPIRY,
                    Currency = CURRENCY,
                    PaymentResult = PAYMENT_RESULT
                });

                context.SaveChanges();

                var response = await client.GetAsync($"/paymentdetails/{PAYMENT_ID}");
                response.EnsureSuccessStatusCode();

                var responseData = JsonConvert.DeserializeObject<GetPaymentDetailsResponseDTO>(await response.Content.ReadAsStringAsync());

                // assert data shows the payment was successful
                Assert.AreEqual(PAYMENT_RESULT, responseData.PaymentResult);
                Assert.AreEqual(AMOUNT, responseData.Amount);
                Assert.AreEqual(CURRENCY, responseData.Currency);
                Assert.AreEqual(CVV, responseData.CVV); // would be masked but that is handled when ADDING to db
                Assert.AreEqual(CARD_NUMBER, responseData.CardNumber);
                Assert.AreEqual(EXPIRY, responseData.Expiry);
            }
        }

        [TestMethod]
        public async Task Return_200_With_Data_For_Successful_Find_But_Failed_Payment()
        {
            const int PAYMENT_ID = 1;
            const decimal AMOUNT = .1m;
            const string CARD_NUMBER = "123123423543";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            const bool PAYMENT_RESULT = false;
            var EXPIRY = new DateTime(2021, 01, 01);

            var (_, client, context) = SetupServer();

            using (context)
            {
                // add to DB so we can get it back
                context.ProcessedPayments.Add(new ProcessedPayment
                {
                    Id = PAYMENT_ID,
                    Amount = AMOUNT,
                    CardNumber = CARD_NUMBER,
                    CVV = CVV,
                    Expiry = EXPIRY,
                    Currency = CURRENCY,
                    PaymentResult = PAYMENT_RESULT
                });

                context.SaveChanges();

                var response = await client.GetAsync($"/paymentdetails/{PAYMENT_ID}");
                response.EnsureSuccessStatusCode();

                var responseData = JsonConvert.DeserializeObject<GetPaymentDetailsResponseDTO>(await response.Content.ReadAsStringAsync());

                // assert data shows the payment was successful
                Assert.AreEqual(PAYMENT_RESULT, responseData.PaymentResult);
                Assert.AreEqual(AMOUNT, responseData.Amount);
                Assert.AreEqual(CURRENCY, responseData.Currency);
                Assert.AreEqual(CVV, responseData.CVV); // would be masked but that is handled when ADDING to db
                Assert.AreEqual(CARD_NUMBER, responseData.CardNumber);
                Assert.AreEqual(EXPIRY, responseData.Expiry);
            }
        }

        [TestMethod]
        public async Task Return_400_For_Unsuccessful_Find()
        {
            const int PAYMENT_ID = 1;

            var (_, client, _) = SetupServer();

            var response = await client.GetAsync($"/paymentdetails/{PAYMENT_ID}");

            Assert.AreEqual(400, (int)response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ErrorResponseDTO>(content);

            Assert.AreEqual($"No payment details could be found for id {PAYMENT_ID}", data.Message);
            Assert.AreEqual(0, data.Errors.Count);
        }

        [TestMethod]
        public async Task Returns_401_For_UnAuthed_Requests()
        {
            Assert.Fail();
        }
    }
}
