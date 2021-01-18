using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Tests.Requests.Queries.GetPaymentDetails
{
    [TestClass]
    [TestCategory("Requests - Queries - GetPaymentDetails - Validation")]
    public class GetPaymentDetailsValidationTests
    {
        [TestMethod]
        public void Fail_No_Payment_Id()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Pass_Has_Payment_Id()
        {
            Assert.Fail();
        }
    }
}
