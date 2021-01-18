using CheckoutPaymentAPI.Requests.Commands.ProcessPayment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation.TestHelper;
using System.Linq;
using CheckoutPaymentAPI.Core.Providers;

namespace CheckoutPaymentAPI.Tests.Requests.Commands.ProcessPayment
{
    [TestClass]
    [TestCategory("Requests - Commands - ProcessPayment - Validation")]
    public class ProcessPaymentValidationTests
    {
        [TestMethod]
        public void Fails_No_Card_Number()
        {
            var request = new ProcessPaymentRequest();
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.CardNumber, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("Card number required", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Fails_No_Expiry()
        {
            var request = new ProcessPaymentRequest();
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.Expiry, request);

            Assert.IsTrue(failures.Any());
            Assert.AreEqual("Expiry required", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Fails_No_Amount()
        {
            var request = new ProcessPaymentRequest();
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.Amount, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("Amount required", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Fails_No_Currency()
        {
            var request = new ProcessPaymentRequest();
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.Currency, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("Currency required", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Fails_No_CVV()
        {
            var request = new ProcessPaymentRequest();
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.CVV, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("CVV required", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Fails_Expiry_In_Past()
        {
            var testNow = new DateTime(2021, 01, 01);
            var request = new ProcessPaymentRequest
            {
                Expiry = testNow.AddSeconds(-1) // a passed date should not be valid
            };
            var nowProvider = new NowProvider(testNow);
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.Expiry, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("Expiry invalid", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Fails_Zero_Amount()
        {
            var request = new ProcessPaymentRequest
            {
                Amount = 0
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.Amount, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("Amount required", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Fails_CVV_2_Digits()
        {
            var request = new ProcessPaymentRequest
            {
                CVV = "12"
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.CVV, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("CVV invalid", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Fails_CVV_4_Digits()
        {
            var request = new ProcessPaymentRequest 
            { 
                CVV = "1234"
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.CVV, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("CVV invalid", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Passes_Good_Card_Number()
        {
            var request = new ProcessPaymentRequest
            {
                CardNumber = "4111111111111111"
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            validator.ShouldNotHaveValidationErrorFor(r => r.CardNumber, request);
        }

        [TestMethod]
        public void Passes_In_Future_Expiry()
        {
            var request = new ProcessPaymentRequest
            {
                Expiry = DateTime.Now.AddMonths(3).AddDays(-1) // date should be last day of 2 months into the future
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            validator.ShouldNotHaveValidationErrorFor(r => r.Expiry, request);
        }

        [TestMethod]
        public void Passes_Negative_Amount()
        {
            var request = new ProcessPaymentRequest
            {
                Amount = -1
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            validator.ShouldNotHaveValidationErrorFor(r => r.Amount, request);
        }

        [TestMethod]
        public void Passes_Positive_Amount()
        {
            var request = new ProcessPaymentRequest
            {
                Amount = 1
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            validator.ShouldNotHaveValidationErrorFor(r => r.Amount, request);
        }

        [TestMethod]
        public void Passes_Currency_Code()
        {
            var request = new ProcessPaymentRequest 
            {
                Currency = "GBP"
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            validator.ShouldNotHaveValidationErrorFor(r => r.Currency, request);
        }

        [TestMethod]
        public void Passes_CVV_3_Digit()
        {
            var request = new ProcessPaymentRequest 
            {
                CVV = "123"
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            validator.ShouldNotHaveValidationErrorFor(r => r.CVV, request);
        }
    }
}
