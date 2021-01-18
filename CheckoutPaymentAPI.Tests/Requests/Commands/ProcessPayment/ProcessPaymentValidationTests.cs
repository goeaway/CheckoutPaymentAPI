using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Tests.Requests.Commands.ProcessPayment
{
    [TestClass]
    [TestCategory("Requests - Commands - ProcessPayment - Validation")]
    public class ProcessPaymentValidationTests
    {
        [TestMethod]
        public void Fails_No_Card_Number()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Fails_No_Expiry()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Fails_No_Amount()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Fails_No_Currency()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Fails_No_CVV()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Fails_Card_Number_Arbitrary_Problem()
        {
            // don't want to implement all card number issues possible, but show that I would do so in Prod
            Assert.Fail();
        }

        [TestMethod]
        public void Fails_Expiry_In_Past()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Fails_CVV_Not_3_Digits()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Passes_Good_DTO()
        {
            Assert.Fail();
        }
    }
}
