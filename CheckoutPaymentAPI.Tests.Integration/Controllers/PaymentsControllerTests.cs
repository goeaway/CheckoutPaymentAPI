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

namespace CheckoutPaymentAPI.IntegrationTests.Controllers
{
    [TestClass]
    [TestCategory("Integration - PaymentsController")]
    public class PaymentsControllerTests
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
        public async Task Returns_200_With_Success_Data_For_Successful_Payment()
        {
            var (_, client, context) = SetupServer();

            var request = new ProcessPaymentsRequestDTO
            {

            };

            var content = new StringContent(JsonConvert.SerializeObject(request));

            var response = await client.PostAsync("/payments/process", content);

            response.EnsureSuccessStatusCode();

            // assert response has payment id in it
        }

        [TestMethod]
        public async Task Returns_200_With_Fail_Data_For_Failed_Payment()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task Returns_400_For_Failed_Validation()
        {
            const decimal AMOUNT = .1m;
            const string INVALID_CARD_NUMBER = "4111111111111112";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            var EXPIRY = new DateTime(2021, 01, 01);

            var (_, client, context) = SetupServer();

            using (context)
            {
                // add to DB so we can get it back
                context.SaveChanges();

                var request = new ProcessPaymentsRequestDTO
                {
                    Amount = AMOUNT,
                    CardNumber = INVALID_CARD_NUMBER,
                    CVV = CVV,
                    Expiry = EXPIRY,
                    Currency = CURRENCY,
                };

                var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"/payments/process", requestContent);
                Assert.AreEqual(400, (int)response.StatusCode);

                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ErrorResponseDTO>(responseContent);

                Assert.AreEqual("Validation error", data.Message);
                Assert.AreEqual(1, data.Errors.Count);
            }
        }

        [TestMethod]
        public async Task Multiple_Same_Requests_Are_Blocked()
        {
            // api should cache a hash of the data in each process request and block any that are the same as currently live cache records
            Assert.Fail();
        }
    }
}
