using CheckoutPaymentAPI.Models.DTOs;
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

namespace CheckoutPaymentAPI.IntegrationTests.Controllers
{
    [TestClass]
    [TestCategory("Integration - PaymentDetailsController")]
    public class PaymentDetailsControllerTests
    {
        private (TestServer server, HttpClient client) Setup()
        {
            var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            var client = server.CreateClient();

            return (server, client);
        }

        [TestMethod]
        public async Task Return_404_For_No_Id()
        {
            var (_, client) = Setup();

            var response = await client.GetAsync("/paymentdetails");

            Assert.AreEqual(404, (int)response.StatusCode);
        }

        [TestMethod]
        public async Task Return_200_With_Data_For_Successful_Find_And_Payment()
        {
            var (_, client) = Setup();

            var response = await client.GetAsync("/paymentdetails");
            response.EnsureSuccessStatusCode();

            // assert data shows the payment was successful
        }

        [TestMethod]
        public async Task Return_200_With_Data_For_Successful_Find_But_Failed_Payment()
        {
            var (_, client) = Setup();

            var response = await client.GetAsync("/paymentdetails");
            response.EnsureSuccessStatusCode();

            // assert data shows the payment was unsuccessful
        }

        [TestMethod]
        public async Task Return_400_For_Unsuccessful_Find()
        {
            const int PAYMENT_ID = 1;

            var (_, client) = Setup();

            var response = await client.GetAsync($"/paymentdetails/{PAYMENT_ID}");

            Assert.AreEqual(400, (int)response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ErrorResponseDTO>(content);

            Assert.AreEqual($"No payment details could be found for id {PAYMENT_ID}", data.Message);
            Assert.AreEqual(0, data.Errors.Count);
        }
    }
}
