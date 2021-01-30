using CheckoutPaymentAPI.Models.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using CheckoutPaymentAPI.Tests.Core;
using CheckoutPaymentAPI.Models.Entities;
using System.Linq;

namespace CheckoutPaymentAPI.IntegrationTests.Controllers
{
    [TestClass]
    [TestCategory("API - Integration - Get Payment Details")]
    public class GetPaymentDetailsTests
    {
        [TestMethod]
        public async Task Return_404_For_No_Id()
        {
            var (_, client, _) = Setup.CreateServer();

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
            const string OWNER = "CheckoutPaymentAPIClient";
            const string PAYMENT_RESULT = "Success";
            var EXPIRY = new DateTime(2021, 01, 01);

            var (_, client, context) = Setup.CreateServer();

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
                    PaymentResult = PAYMENT_RESULT,
                    Owner = OWNER
                });

                context.SaveChanges();
                client.DefaultRequestHeaders.Add("X-API-KEY", "CheckoutPaymentAPI-Q2hlY2tvdXRQYXltZW50QVBJ");

                var response = await client.GetAsync($"/payments/{PAYMENT_ID}");
                response.EnsureSuccessStatusCode();

                var responseData = JsonConvert.DeserializeObject<GetPaymentDetailsResponseDTO>(await response.Content.ReadAsStringAsync());

                // assert data shows the payment was successful
                Assert.AreEqual(PAYMENT_RESULT, responseData.PaymentResult);
                Assert.AreEqual(AMOUNT, responseData.Amount);
                Assert.AreEqual(CURRENCY, responseData.Currency);
                Assert.AreEqual(CVV, responseData.CVV); // would be masked but that is handled when ADDING to db
                Assert.AreEqual(CARD_NUMBER, responseData.CardNumber);

                Assert.AreEqual(EXPIRY.Year, responseData.Expiry.Year);
                Assert.AreEqual(EXPIRY.Month, responseData.Expiry.Month);
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
            const string PAYMENT_RESULT = "Payment Declined";
            const string OWNER = "CheckoutPaymentAPIClient";
            var EXPIRY = new DateTime(2021, 01, 01);

            var (_, client, context) = Setup.CreateServer();

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
                    PaymentResult = PAYMENT_RESULT,
                    Owner = OWNER
                });

                context.SaveChanges();
                client.DefaultRequestHeaders.Add("X-API-KEY", "CheckoutPaymentAPI-Q2hlY2tvdXRQYXltZW50QVBJ");

                var response = await client.GetAsync($"/payments/{PAYMENT_ID}");
                response.EnsureSuccessStatusCode();

                var responseData = JsonConvert.DeserializeObject<GetPaymentDetailsResponseDTO>(await response.Content.ReadAsStringAsync());

                // assert data shows the payment was successful
                Assert.AreEqual(PAYMENT_RESULT, responseData.PaymentResult);
                Assert.AreEqual(AMOUNT, responseData.Amount);
                Assert.AreEqual(CURRENCY, responseData.Currency);
                Assert.AreEqual(CVV, responseData.CVV); // would be masked but that is handled when ADDING to db
                Assert.AreEqual(CARD_NUMBER, responseData.CardNumber);
                Assert.AreEqual(EXPIRY.Year, responseData.Expiry.Year);
                Assert.AreEqual(EXPIRY.Month, responseData.Expiry.Month);
            }
        }

        [TestMethod]
        public async Task Return_404_For_Unsuccessful_Find()
        {
            const int PAYMENT_ID = 1;

            var (_, client, _) = Setup.CreateServer();

            client.DefaultRequestHeaders.Add("X-API-KEY", "CheckoutPaymentAPI-Q2hlY2tvdXRQYXltZW50QVBJ");
            var response = await client.GetAsync($"/payments/{PAYMENT_ID}");

            Assert.AreEqual(404, (int)response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ErrorResponseDTO>(content);

            Assert.AreEqual($"Payment details not found", data.Message);
            Assert.AreEqual(0, data.Errors.Count());
        }

        [TestMethod]
        public async Task Return_404_For_Incorrect_Owner()
        {
            const int PAYMENT_ID = 1;
            const decimal AMOUNT = .1m;
            const string CARD_NUMBER = "123123423543";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            const string PAYMENT_RESULT = "Payment Declined";
            const string OWNER = "DUMMY CLIENT";
            var EXPIRY = new DateTime(2021, 01, 01);

            var (_, client, context) = Setup.CreateServer();

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
                    PaymentResult = PAYMENT_RESULT,
                    Owner = OWNER
                });

                context.SaveChanges();
                client.DefaultRequestHeaders.Add("X-API-KEY", "CheckoutPaymentAPI-Q2hlY2tvdXRQYXltZW50QVBJ");

                var response = await client.GetAsync($"/payments/{PAYMENT_ID}");
                Assert.AreEqual(404, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task Returns_401_For_UnAuthed_Requests()
        {
            const int PAYMENT_ID = 1;

            var (_, client, context) = Setup.CreateServer();

            using (context)
            {
                client.DefaultRequestHeaders.Add("X-API-KEY", "CheckoutPaymentAPI-WrongAPIKey");

                var response = await client.GetAsync($"/payments/{PAYMENT_ID}");
                Assert.AreEqual(401, (int)response.StatusCode);
            }
        }
    }
}
