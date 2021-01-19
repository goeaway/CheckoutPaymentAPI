using CheckoutPaymentAPI.Models.DTOs;
using Microsoft.AspNetCore.Hosting;
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
    [TestCategory("Integration - PaysmentsController")]
    public class PaymentsControllerTests
    {
        private (TestServer server, HttpClient client) Setup()
        {
            var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            var client = server.CreateClient();

            return (server, client);
        }

        [TestMethod]
        public async Task Returns_200_With_Success_Data_For_Successful_Payment()
        {
            var (_, client) = Setup();

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
            Assert.Fail();
        }
    }
}
