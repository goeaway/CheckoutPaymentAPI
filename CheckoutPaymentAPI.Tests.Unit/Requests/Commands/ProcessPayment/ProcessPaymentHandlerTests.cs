using CheckoutPaymentAPI.Core.Abstractions;
using CheckoutPaymentAPI.Core.Providers;
using CheckoutPaymentAPI.Persistence;
using CheckoutPaymentAPI.Requests.Commands.ProcessPayment;
using CheckoutPaymentAPI.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CheckoutPaymentAPI.Core.Models;

namespace CheckoutPaymentAPI.Tests.Requests.Commands.ProcessPayment
{
    [TestClass]
    [TestCategory("Requests - Commands - ProcessPayment - Handler")]
    public class ProcessPaymentHandlerTests
    {
        [TestMethod]
        public async Task Stores_Masked_Card_Details()
        {
            const int RETURNED_PAYMENT_ID = 1;
            const string CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            const decimal AMOUNT = .1m;
            var EXPIRY = new DateTime(2021, 01, 01);

            var testNow = new DateTime(2021, 02, 01);
            var nowProvider = new NowProvider(testNow);

            var request = new ProcessPaymentRequest
            {
                CardNumber = CARD_NUMBER,
                Amount = AMOUNT,
                Currency = CURRENCY,
                CVV = CVV,
                Expiry = EXPIRY
            };

            var acqBankMock = new Mock<IAcquiringBank>();
            acqBankMock
                .Setup(mock => mock.SendPayment())
                .ReturnsAsync(new AcquiringBankResponse
                {
                    Success = true,
                    PaymentId = RETURNED_PAYMENT_ID
                });


            using var context = Setup.CreateContext();

            var handler = new ProcessPaymentHandler(acqBankMock.Object, nowProvider, context);
            var result = await handler.Handle(request, CancellationToken.None);

            Assert.IsTrue(result.Success);

            // assert that a record is in the db for this payment id,
            var foundPayment = context.ProcessedPayments.Find(result.PaymentId);

            // should be masked
            Assert.AreEqual("************1111", foundPayment.CardNumber);
            Assert.AreEqual("***", foundPayment.CVV);
            Assert.AreEqual(EXPIRY, foundPayment.Expiry);
            Assert.AreEqual(AMOUNT, foundPayment.Amount);
            Assert.AreEqual(CURRENCY, foundPayment.Currency);
        }

        [TestMethod]
        public async Task Error_In_Acquiring_Bank_Returns_Unsuccessful_Response()
        {
            const int RETURNED_PAYMENT_ID = 1;
            const string CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            const decimal AMOUNT = .1m;
            var EXPIRY = new DateTime(2021, 01, 01);

            var testNow = new DateTime(2021, 02, 01);
            var nowProvider = new NowProvider(testNow);

            var request = new ProcessPaymentRequest
            {
                CardNumber = CARD_NUMBER,
                Amount = AMOUNT,
                Currency = CURRENCY,
                CVV = CVV,
                Expiry = EXPIRY
            };

            using var context = Setup.CreateContext();

            var acqBankMock = new Mock<IAcquiringBank>();
            acqBankMock
                .Setup(mock => mock.SendPayment())
                .ReturnsAsync(new AcquiringBankResponse
                {
                    Success = false,
                    PaymentId = RETURNED_PAYMENT_ID
                });

            // use mock of acqbank to ensure we get a failure here
            var handler = new ProcessPaymentHandler(acqBankMock.Object, nowProvider, context);
            var result = await handler.Handle(request, CancellationToken.None);

            // ensure payment is still saved in failed state
            var foundPayment = context.ProcessedPayments.Find(result.PaymentId);

            Assert.AreEqual(RETURNED_PAYMENT_ID, result.PaymentId);
            Assert.IsFalse(result.Success);

            Assert.AreEqual("************1111", foundPayment.CardNumber);
            Assert.AreEqual("***", foundPayment.CVV);
            Assert.AreEqual(EXPIRY, foundPayment.Expiry);
            Assert.AreEqual(AMOUNT, foundPayment.Amount);
            Assert.AreEqual(CURRENCY, foundPayment.Currency);
        }
    }
}
