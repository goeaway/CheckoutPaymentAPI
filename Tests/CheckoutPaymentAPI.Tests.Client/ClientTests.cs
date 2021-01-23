using CheckoutPaymentAPI.Persistence;
using CheckoutPaymentAPI.Tests.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using CheckoutPaymentAPI.Client;
using System;
using CheckoutPaymentAPI.Persistence.Models;
using CheckoutPaymentAPI.Core.Providers;

namespace CheckoutPaymentAPI.Tests.Client
{
    [TestClass]
    [TestCategory("Client - CheckoutPaymentAPIClient")]
    public class ClientTests
    {
        private const string API_KEY = "CheckoutPaymentAPI-Q2hlY2tvdXRQYXltZW50QVBJ";
        private const string API_KEY_OWNER = "CheckoutPaymentAPIClient";

        private (TestServer, HttpClient, CheckoutPaymentAPIContext) SetupServer(DateTime? now = null)
        {
            var context = Setup.CreateContext();

            var server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    services.AddSingleton(context);

                    if(now.HasValue)
                    {
                        services.AddSingleton(new NowProvider(now.Value));
                    }
                }));
            var client = server.CreateClient();
            return (server, client, context);
        }

        [TestMethod]
        public async Task ProcessPayment_Can_Process()
        {
            const decimal AMOUNT = .1m;
            const string CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            var testNow = new DateTime(2021, 01, 01);
            var EXPIRY = testNow.AddYears(1);

            var (_, client, context) = SetupServer(testNow);

            var apiClient = new APIClient(client, API_KEY);

            var response = await apiClient.ProcessPayment(
                CARD_NUMBER,
                EXPIRY,
                AMOUNT,
                CURRENCY,
                CVV
            );

            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsNull(response.Error);
            Assert.IsTrue(response.Data.Success);
            Assert.IsNotNull(response.Data.PaymentId);

            using (context)
            {
                var found = context.ProcessedPayments.Find(response.Data.PaymentId);
                Assert.IsNotNull(found);
            }
        }

        [TestMethod]
        public async Task ProcessPayment_Failed_Requests_Dont_Throw()
        {
            const decimal AMOUNT = .1m;
            const string CARD_NUMBER = "123123423543";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            var testNow = new DateTime(2021, 01, 01);
            var EXPIRY = testNow.AddYears(1);

            var (_, client, _) = SetupServer(testNow);

            var apiClient = new APIClient(client, "WRONG API KEY");

            var response = await apiClient.ProcessPayment(
                CARD_NUMBER,
                EXPIRY,
                AMOUNT,
                CURRENCY,
                CVV
            );

            Assert.AreEqual(401, (int)response.StatusCode);
            Assert.IsNull(response.Data);
            Assert.IsNull(response.Error);
        }

        [TestMethod]
        public async Task ProcessPayment_Throws_No_Card_Number()
        {
            const decimal AMOUNT = .1m;
            const string CVV = "123";
            const string CURRENCY = "GBP";
            var testNow = new DateTime(2021, 01, 01);
            var EXPIRY = testNow.AddYears(1);

            var (_, client, _) = SetupServer();

            var apiClient = new APIClient(client, API_KEY);

            await Assert
                .ThrowsExceptionAsync<ArgumentNullException>(
                    () => apiClient.ProcessPayment(null, 
                        EXPIRY,
                        AMOUNT,
                        CURRENCY,
                        CVV));
        }

        [TestMethod]
        public async Task ProcessPayment_Throws_No_Currency()
        {
            const string CARD_NUMBER = "";
            const decimal AMOUNT = .1m;
            const string CVV = "123";
            var testNow = new DateTime(2021, 01, 01);
            var EXPIRY = testNow.AddYears(1);

            var (_, client, _) = SetupServer();

            var apiClient = new APIClient(client, API_KEY);

            await Assert
                .ThrowsExceptionAsync<ArgumentNullException>(
                    () => apiClient.ProcessPayment(CARD_NUMBER,
                        EXPIRY,
                        AMOUNT,
                        null,
                        CVV));
        }

        [TestMethod]
        public async Task ProcessPayment_Throws_No_CVV()
        {
            const string CARD_NUMBER = "";
            const decimal AMOUNT = .1m;
            const string CURRENCY = "GBP";
            var testNow = new DateTime(2021, 01, 01);
            var EXPIRY = testNow.AddYears(1);

            var (_, client, _) = SetupServer();

            var apiClient = new APIClient(client, API_KEY);

            await Assert
                .ThrowsExceptionAsync<ArgumentNullException>(
                    () => apiClient.ProcessPayment(CARD_NUMBER,
                        EXPIRY,
                        AMOUNT,
                        CURRENCY,
                        null));
        }

        [TestMethod]
        public async Task GetPaymentDetails_Can_Get_Details()
        {
            const int PAYMENT_ID = 1;
            const decimal AMOUNT = .1m;
            const string CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            var testNow = new DateTime(2021, 01, 01);
            var EXPIRY = testNow.AddYears(1);
            const bool PAYMENT_RESULT = true;

            var (_, client, context) = SetupServer(testNow);

            using (context)
            {
                context.ProcessedPayments.Add(new ProcessedPayment
                {
                    Id = PAYMENT_ID,
                    Amount = AMOUNT,
                    CardNumber = CARD_NUMBER,
                    CVV = CVV,
                    Created = new DateTime(2021, 01, 01),
                    Currency = CURRENCY,
                    Expiry = EXPIRY,
                    PaymentResult = PAYMENT_RESULT,
                    Owner = API_KEY_OWNER
                });

                context.SaveChanges();

                var apiClient = new APIClient(client, API_KEY);

                var response = await apiClient.GetPaymentDetails(PAYMENT_ID);

                Assert.AreEqual(200, (int)response.StatusCode);
                Assert.IsNull(response.Error);

                Assert.AreEqual(PAYMENT_RESULT, response.Data.PaymentResult);
                Assert.AreEqual(CARD_NUMBER, response.Data.CardNumber);
                Assert.AreEqual(CVV, response.Data.CVV);
                Assert.AreEqual(CURRENCY, response.Data.Currency);
                Assert.AreEqual(EXPIRY, response.Data.Expiry);
                Assert.AreEqual(AMOUNT, response.Data.Amount);
            }
        }

        [TestMethod]
        public async Task GetPaymentDetails_Failed_Requests_Dont_Throw()
        {
            const int PAYMENT_ID = 1;

            var (_, client, _) = SetupServer();

            var apiClient = new APIClient(client, API_KEY);

            var response = await apiClient.GetPaymentDetails(PAYMENT_ID);

            Assert.AreEqual(404, (int)response.StatusCode);
            Assert.AreEqual("Payment details not found", response.Error.Message);
            Assert.AreEqual(0, response.Error.Errors.Count);
        }
    }
}
