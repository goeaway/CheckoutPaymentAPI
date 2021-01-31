using CheckoutPaymentAPI.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using CheckoutPaymentAPI.Client;
using System;
using CheckoutPaymentAPI.Models.Entities;

namespace CheckoutPaymentAPI.Tests.Client
{
    [TestClass]
    [TestCategory("Client - CheckoutPaymentAPIClient")]
    public class ClientTests
    {
        private const string API_KEY = "CheckoutPaymentAPI-Q2hlY2tvdXRQYXltZW50QVBJ";
        private const string API_KEY_OWNER = "CheckoutPaymentAPIClient";

        [TestMethod]
        public async Task ProcessPayment_Can_Process()
        {
            const decimal AMOUNT = .1m;
            const string CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            var testNow = new DateTime(2021, 01, 01);
            var expiryDate = testNow.AddYears(1);
            var EXPIRY = new MonthYear(expiryDate.Month, expiryDate.Year);

            var (_, client, context) = Setup.CreateServer(new Setup.CreateServerOptions
            {
                TestNow = testNow
            });

            var apiClient = new ApiClient(client, API_KEY);

            var response = await apiClient.ProcessPayment(
                CARD_NUMBER,
                CVV,
                EXPIRY,
                AMOUNT,
                CURRENCY
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
        public async Task ProcessPayment_Returns_400_For_Validation_Failure()
        {
            const decimal AMOUNT = .1m;
            const string CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            var testNow = new DateTime(2021, 01, 01);
            var expiryDate = testNow.AddSeconds(-1);
            var EXPIRY = new MonthYear(expiryDate.Month, expiryDate.Year);

            var (_, client, _) = Setup.CreateServer(new Setup.CreateServerOptions
            {
                TestNow = testNow
            });

            var apiClient = new ApiClient(client, API_KEY);

            var response = await apiClient.ProcessPayment(
                CARD_NUMBER,
                CVV,
                EXPIRY,
                AMOUNT,
                CURRENCY
            );

            Assert.AreEqual(400, (int)response.StatusCode);
            Assert.IsNull(response.Data);
            Assert.IsNotNull(response.Error);
        }

        [TestMethod]
        public async Task ProcessPayment_Failed_Requests_Dont_Throw()
        {
            const decimal AMOUNT = .1m;
            const string CARD_NUMBER = "123123423543";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            var testNow = new DateTime(2021, 01, 01);
            var expiryDate = testNow.AddYears(1);
            var EXPIRY = new MonthYear(expiryDate.Month, expiryDate.Year);

            var (_, client, _) = Setup.CreateServer(new Setup.CreateServerOptions
            {
                TestNow = testNow
            });

            var apiClient = new ApiClient(client, "WRONG API KEY");

            var response = await apiClient.ProcessPayment(
                CARD_NUMBER,
                CVV,
                EXPIRY,
                AMOUNT,
                CURRENCY
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
            var expiryDate = testNow.AddYears(1);
            var EXPIRY = new MonthYear(expiryDate.Month, expiryDate.Year);

            var (_, client, _) = Setup.CreateServer(new Setup.CreateServerOptions
            {
                TestNow = testNow
            });

            var apiClient = new ApiClient(client, API_KEY);

            await Assert
                .ThrowsExceptionAsync<ArgumentNullException>(
                    () => apiClient.ProcessPayment(null,
                        CVV,
                        EXPIRY,
                        AMOUNT,
                        CURRENCY));
        }

        [TestMethod]
        public async Task ProcessPayment_Throws_No_Currency()
        {
            const string CARD_NUMBER = "";
            const decimal AMOUNT = .1m;
            const string CVV = "123";
            var testNow = new DateTime(2021, 01, 01);
            var expiryDate = testNow.AddYears(1);
            var EXPIRY = new MonthYear(expiryDate.Month, expiryDate.Year);

            var (_, client, _) = Setup.CreateServer(new Setup.CreateServerOptions
            {
                TestNow = testNow
            });

            var apiClient = new ApiClient(client, API_KEY);

            await Assert
                .ThrowsExceptionAsync<ArgumentNullException>(
                    () => apiClient.ProcessPayment(CARD_NUMBER,
                        CVV,
                        EXPIRY,
                        AMOUNT,
                        null));
        }

        [TestMethod]
        public async Task ProcessPayment_Throws_No_CVV()
        {
            const string CARD_NUMBER = "";
            const decimal AMOUNT = .1m;
            const string CURRENCY = "GBP";
            var testNow = new DateTime(2021, 01, 01);
            var expiryDate = testNow.AddYears(1);
            var EXPIRY = new MonthYear(expiryDate.Month, expiryDate.Year);

            var (_, client, _) = Setup.CreateServer(new Setup.CreateServerOptions
            {
                TestNow = testNow
            });

            var apiClient = new ApiClient(client, API_KEY);

            await Assert
                .ThrowsExceptionAsync<ArgumentNullException>(
                    () => apiClient.ProcessPayment(CARD_NUMBER,
                        null,
                        EXPIRY,
                        AMOUNT,
                        CURRENCY));
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
            const string PAYMENT_RESULT = "Success";

            var (_, client, context) = Setup.CreateServer(new Setup.CreateServerOptions
            {
                TestNow = testNow
            });

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

                var apiClient = new ApiClient(client, API_KEY);

                var response = await apiClient.GetPaymentDetails(PAYMENT_ID);

                Assert.AreEqual(200, (int)response.StatusCode);
                Assert.IsNull(response.Error);

                Assert.AreEqual(PAYMENT_RESULT, response.Data.PaymentResult);
                Assert.AreEqual(CARD_NUMBER, response.Data.CardNumber);
                Assert.AreEqual(CVV, response.Data.CVV);
                Assert.AreEqual(CURRENCY, response.Data.Currency);
                Assert.AreEqual(AMOUNT, response.Data.Amount);
                Assert.AreEqual(EXPIRY.Year, response.Data.Expiry.Year);
                Assert.AreEqual(EXPIRY.Month, response.Data.Expiry.Month);
            }
        }

        [TestMethod]
        public async Task GetPaymentDetails_Failed_Requests_Dont_Throw()
        {
            const int PAYMENT_ID = 1;

            var (_, client, _) = Setup.CreateServer();

            var apiClient = new ApiClient(client, API_KEY);

            var response = await apiClient.GetPaymentDetails(PAYMENT_ID);

            Assert.AreEqual(404, (int)response.StatusCode);
            Assert.AreEqual("Payment details not found", response.Error.Message);
            Assert.AreEqual(0, response.Error.Errors.Count);
        }
    }
}
