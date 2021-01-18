using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Tests.Requests.Commands.ProcessPayment
{
    [TestClass]
    [TestCategory("Requests - Commands - ProcessPayment - Handler")]
    public class ProcessPaymentHandlerTests
    {
        [TestMethod]
        public async Task Stores_Masked_Card_Details()
        {
            // storage must mask sensitive data
            Assert.Fail();
        }

        [TestMethod]
        public async Task Uses_Acquiring_Bank_Service()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task Multiple_Same_Requests_In_Time_Period_Throw()
        {
            // talk about arbitrary choice on time
            Assert.Fail();
        }
    }
}
