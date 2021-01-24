using CheckoutPaymentAPI.Application.Exceptions;
using CheckoutPaymentAPI.Application.Requests.Queries.GetPaymentDetails;
using CheckoutPaymentAPI.Models.Entities;
using CheckoutPaymentAPI.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Tests.Requests.Queries.GetPaymentDetails
{
    [TestClass]
    [TestCategory("API - Unit - Queries - GetPaymentDetails - Handler")]
    public class GetPaymentHandlerTests
    {
        private readonly ILogger _logger = new LoggerConfiguration().CreateLogger();

        [TestMethod]
        public async Task Throws_If_No_Data_Found_For_Id()
        {
            var request = new GetPaymentDetailsRequest
            {
                PaymentId = 1
            };

            using var context = Setup.CreateContext();
            var handler = new GetPaymentDetailsHandler(_logger, context);
            await Assert
                .ThrowsExceptionAsync<RequestFailedException>(
                    () => handler.Handle(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task Throws_If_No_Data_Found_For_Owner()
        {
            const int PAYMENT_ID = 1;
            const string CARD_NUMBER = "1234";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            const decimal AMOUNT = .1m;
            const bool PAYMENT_RESULT = true;
            const string OWNER = "owner";
            var EXPIRY = new DateTime(2021, 01, 01);

            var request = new GetPaymentDetailsRequest
            {
                PaymentId = PAYMENT_ID,
                Owner = OWNER
            };

            using var context = Setup.CreateContext();

            context.ProcessedPayments.Add(new ProcessedPayment
            {
                Id = PAYMENT_ID,
                CardNumber = CARD_NUMBER,
                Expiry = EXPIRY,
                Currency = CURRENCY,
                Amount = AMOUNT,
                CVV = CVV,
                PaymentResult = PAYMENT_RESULT,
                Owner = "Different Owner"
            });

            context.SaveChanges();

            var handler = new GetPaymentDetailsHandler(_logger, context);
            await Assert
                .ThrowsExceptionAsync<RequestFailedException>(
                    () => handler.Handle(request, CancellationToken.None));
        }


        [TestMethod]
        public async Task Returns_Details_In_DTO()
        {
            const int PAYMENT_ID = 1;
            const string CARD_NUMBER = "1234";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            const decimal AMOUNT = .1m;
            const bool PAYMENT_RESULT = true;
            const string OWNER = "owner";
            var EXPIRY = new DateTime(2021, 01, 01);

            var request = new GetPaymentDetailsRequest
            {
                PaymentId = PAYMENT_ID,
                Owner = OWNER
            };

            using var context = Setup.CreateContext();

            context.ProcessedPayments.Add(new ProcessedPayment
            {
                Id = PAYMENT_ID,
                CardNumber = CARD_NUMBER,
                Expiry = EXPIRY,
                Currency = CURRENCY,
                Amount = AMOUNT,
                CVV = CVV,
                PaymentResult = PAYMENT_RESULT,
                Owner = OWNER
            });

            context.SaveChanges();

            var handler = new GetPaymentDetailsHandler(_logger, context);
            var result = await handler.Handle(request, CancellationToken.None);

            Assert.AreEqual(PAYMENT_RESULT, result.PaymentResult);
            Assert.AreEqual(CARD_NUMBER, result.CardNumber);
            Assert.AreEqual(CVV, result.CVV);
            Assert.AreEqual(EXPIRY, result.Expiry);
            Assert.AreEqual(CURRENCY, result.Currency);
            Assert.AreEqual(AMOUNT, result.Amount);
        }
    }
}
