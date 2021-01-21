using CheckoutPaymentAPI.Requests.Queries.GetPaymentDetails;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckoutPaymentAPI.Tests.Requests.Queries.GetPaymentDetails
{
    [TestClass]
    [TestCategory("API - Requests - Queries - GetPaymentDetails - Validation")]
    public class GetPaymentDetailsValidationTests
    {
        [TestMethod]
        public void Fail_No_Payment_Id()
        {
            var request = new GetPaymentDetailsRequest();
            var validator = new GetPaymentDetailsValidator();
            var failures = validator.ShouldHaveValidationErrorFor(r => r.PaymentId, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("Payment id required", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Pass_Has_Payment_Id()
        {
            var request = new GetPaymentDetailsRequest 
            {
                PaymentId = 1
            };
            var validator = new GetPaymentDetailsValidator();
            validator.ShouldNotHaveValidationErrorFor(r => r.PaymentId, request);
        }
    }
}
