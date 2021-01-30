using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FluentValidation.TestHelper;
using System.Linq;
using CheckoutPaymentAPI.Application.Requests.Commands.ProcessPayment;
using CheckoutPaymentAPI.Core.Providers;
using CheckoutPaymentAPI.Models;

namespace CheckoutPaymentAPI.Tests.Requests.Commands.ProcessPayment
{
    [TestClass]
    [TestCategory("API - Unit - Commands - ProcessPayment - Validation")]
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
            Assert.AreEqual("Amount must be greater than 0", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Fails_No_Currency()
        {
            var request = new ProcessPaymentRequest();
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.Currency, request);

            Assert.AreNotEqual(0, failures.Count());
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
        public void Fails_No_Owner()
        {
            var request = new ProcessPaymentRequest();
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.Owner, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("Owner required", failures.First().ErrorMessage);
        }

        [TestMethod]
        [DataRow("4111111111111112")]
        [DataRow("411111111111111f")]
        [DataRow("411111111111111.")]
        public void Fails_Invalid_Card_Number(string cardNumber)
        {
            var request = new ProcessPaymentRequest
            {
                CardNumber = cardNumber
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.CardNumber, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("Card number invalid", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Fails_Expiry_In_Past()
        {
            var testNow = new DateTime(2021, 01, 01);
            var expiryDate = testNow.AddSeconds(-1);

            var request = new ProcessPaymentRequest
            {
                Expiry = new MonthYear { Year = expiryDate.Year, Month = expiryDate.Month } // a passed date should not be valid
            };
            var nowProvider = new NowProvider(testNow);
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.Expiry, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("Expiry invalid", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Fails_No_Expiry_Year()
        {
            var testNow = new DateTime(2021, 01, 01);
            var expiryDate = testNow.AddSeconds(-1);

            var request = new ProcessPaymentRequest
            {
                Expiry = new MonthYear { Year = 0, Month = expiryDate.Month } // a passed date should not be valid
            };
            var nowProvider = new NowProvider(testNow);
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.Expiry.Year, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("Expiry year required", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Fails_No_Expiry_Month()
        {
            var testNow = new DateTime(2021, 01, 01);
            var expiryDate = testNow.AddSeconds(-1);

            var request = new ProcessPaymentRequest
            {
                Expiry = new MonthYear { Year = expiryDate.Year, Month = 0 } // a passed date should not be valid
            };
            var nowProvider = new NowProvider(testNow);
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.Expiry.Month, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("Expiry month must be between 1 and 12", failures.First().ErrorMessage);
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
            Assert.AreEqual("Amount must be greater than 0", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Fails_Negative_Amount()
        {
            var request = new ProcessPaymentRequest
            {
                Amount = -1
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.Amount, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("Amount must be greater than 0", failures.First().ErrorMessage);
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
            Assert.AreEqual("CVV must be 3 characters long", failures.First().ErrorMessage);
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
            Assert.AreEqual("CVV must be 3 characters long", failures.First().ErrorMessage);
        }

        [TestMethod]
        [DataRow('.')]
        [DataRow(' ')]
        [DataRow('£')]
        [DataRow('#')]
        [DataRow('!')]
        [DataRow('/')]
        public void Fails_CVV_Symbols(char symbol)
        {
            var request = new ProcessPaymentRequest
            {
                CVV = $"1{symbol}3"
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.CVV, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("CVV must not contain letters or symbols", failures.First().ErrorMessage);
        }

        [TestMethod]
        [DataRow('a')]
        [DataRow('A')]
        public void Fails_CVV_Letters(char letter)
        {
            var request = new ProcessPaymentRequest
            {
                CVV = $"1{letter}3"
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.CVV, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("CVV must not contain letters or symbols", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Fails_Currency_Not_Supported()
        {
            var request = new ProcessPaymentRequest
            {
                Currency = "definitely not in the list"
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            var failures = validator.ShouldHaveValidationErrorFor(r => r.Currency, request);

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("ISO 4217 Currency code not recognised", failures.First().ErrorMessage);
        }

        [TestMethod]
        public void Passes_Valid_Card_Number()
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
            var testNow = new DateTime(2021, 01, 01);
            var expiryDate = testNow.AddMonths(3).AddDays(-1);

            var request = new ProcessPaymentRequest
            {
                Expiry = new MonthYear { Year = expiryDate.Year, Month = expiryDate.Month } // date should be last day of 2 months into the future
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            validator.ShouldNotHaveValidationErrorFor(r => r.Expiry, request);
        }

        [TestMethod]
        public void Passes_Same_Month_Expiry()
        {
            var testNow = new DateTime(2021, 01, 01);

            var request = new ProcessPaymentRequest
            {
                Expiry = new MonthYear { Year = testNow.Year, Month = testNow.Month } // date should be last day of 2 months into the future
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            validator.ShouldNotHaveValidationErrorFor(r => r.Expiry, request);
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

        [TestMethod]
        public void Passes_Owner()
        {
            var request = new ProcessPaymentRequest
            {
                Owner = "Owner"
            };
            var nowProvider = new NowProvider();
            var validator = new ProcessPaymentValidator(nowProvider);
            validator.ShouldNotHaveValidationErrorFor(r => r.Owner, request);
        }
    }
}
