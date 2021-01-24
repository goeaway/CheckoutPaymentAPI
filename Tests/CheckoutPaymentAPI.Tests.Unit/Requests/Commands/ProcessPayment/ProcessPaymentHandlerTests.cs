using CheckoutPaymentAPI.Requests.Commands.ProcessPayment;
using CheckoutPaymentAPI.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using CheckoutPaymentAPI.Models;
using CheckoutPaymentAPI.Options;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using CheckoutPaymentAPI.Exceptions;
using CheckoutPaymentAPI.AcquiringBank;
using CheckoutPaymentAPI.Providers;

namespace CheckoutPaymentAPI.Tests.Requests.Commands.ProcessPayment
{
    [TestClass]
    [TestCategory("API - Unit - Commands - ProcessPayment - Handler")]
    public class ProcessPaymentHandlerTests
    {
        private readonly ILogger _logger = new LoggerConfiguration().CreateLogger();
        private readonly CachingOptions _cachingOptions = new CachingOptions
        {
            ProccessedPaymentTTLMinutes = 1
        };

        [TestMethod]
        public async Task Stores_Masked_Card_Details()
        {
            const int RETURNED_PAYMENT_ID = 1;
            const string CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            const string OWNER = "owner";
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
                Expiry = EXPIRY,
                Owner = OWNER
            };

            var acqBankMock = new Mock<IAcquiringBank>();
            acqBankMock
                .Setup(mock => mock.SendPayment(It.IsAny<AcquiringBankRequest>()))
                .ReturnsAsync(new AcquiringBankResponse
                {
                    Success = true,
                    PaymentId = RETURNED_PAYMENT_ID
                });

            var memoryCacheMock = new Mock<IMemoryCache>();
            var cacheEntryMock = new Mock<ICacheEntry>();

            memoryCacheMock
                .Setup(mock => mock.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);

            memoryCacheMock
                .Setup(mock => mock.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntryMock.Object);

            using var context = Setup.CreateContext();

            var handler = new ProcessPaymentHandler(
                acqBankMock.Object, 
                nowProvider, 
                memoryCacheMock.Object,
                _logger,
                _cachingOptions,
                context);
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
            Assert.AreEqual(OWNER, foundPayment.Owner);
        }

        [TestMethod]
        public async Task Unsuccesful_Bank_Result_Does_Not_Throw()
        {
            const int RETURNED_PAYMENT_ID = 1;
            const string CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            const string OWNER = "owner";
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
                Expiry = EXPIRY,
                Owner = OWNER
            };

            using var context = Setup.CreateContext();

            var acqBankMock = new Mock<IAcquiringBank>();
            acqBankMock
                .Setup(mock => mock.SendPayment(It.IsAny<AcquiringBankRequest>()))
                .ReturnsAsync(new AcquiringBankResponse
                {
                    Success = false,
                    PaymentId = RETURNED_PAYMENT_ID
                });

            var memoryCacheMock = new Mock<IMemoryCache>();
            var cacheEntryMock = new Mock<ICacheEntry>();

            memoryCacheMock
                .Setup(mock => mock.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);

            memoryCacheMock
                .Setup(mock => mock.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntryMock.Object);

            // use mock of acqbank to ensure we get a failure here
            var handler = new ProcessPaymentHandler(
                acqBankMock.Object, 
                nowProvider,
                memoryCacheMock.Object,
                _logger,
                _cachingOptions,
                context);
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
            Assert.AreEqual(OWNER, foundPayment.Owner);
        }
 
        [TestMethod]
        public async Task Passes_Request_Items_To_Acq_Bank_Call()
        {
            const int RETURNED_PAYMENT_ID = 1;
            const string CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            const string OWNER = "owner";
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
                Expiry = EXPIRY,
                Owner = OWNER
            };

            var acqBankMock = new Mock<IAcquiringBank>();
            acqBankMock
                .Setup(mock => mock.SendPayment(It.IsAny<AcquiringBankRequest>()))
                .ReturnsAsync(new AcquiringBankResponse
                {
                    Success = true,
                    PaymentId = RETURNED_PAYMENT_ID
                });

            var memoryCacheMock = new Mock<IMemoryCache>();
            var cacheEntryMock = new Mock<ICacheEntry>();

            memoryCacheMock
                .Setup(mock => mock.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);

            memoryCacheMock
                .Setup(mock => mock.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntryMock.Object);

            using var context = Setup.CreateContext();

            var handler = new ProcessPaymentHandler(
                acqBankMock.Object,
                nowProvider,
                memoryCacheMock.Object,
                _logger,
                _cachingOptions,
                context);
            var result = await handler.Handle(request, CancellationToken.None);

            acqBankMock.Verify(
                mock => mock.SendPayment(
                        It.Is<AcquiringBankRequest>(
                            r => r.CardNumber == CARD_NUMBER && r.Amount == AMOUNT && r.Currency == CURRENCY && r.CVV == CVV && r.Expiry == EXPIRY)
                    ), 
                Times.Once);
        }

        [TestMethod]
        public async Task Throws_When_Cache_Already_Contains_Key()
        {
            const string CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            const string OWNER = "owner";
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
                Expiry = EXPIRY,
                Owner = OWNER
            };

            var acqBankMock = new Mock<IAcquiringBank>();
            var memoryCacheMock = new Mock<IMemoryCache>();

            memoryCacheMock
                .Setup(mock => mock.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(true);

            using var context = Setup.CreateContext();

            var handler = new ProcessPaymentHandler(
                acqBankMock.Object,
                nowProvider,
                memoryCacheMock.Object,
                _logger,
                _cachingOptions,
                context);

            await Assert
                .ThrowsExceptionAsync<RequestFailedException>(
                    () => handler.Handle(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task Adds_Key_To_Cache()
        {
            const string GENERATED_REQUEST_KEY = "NxksOI06dqKBd1Dqrb8QaGP3n4aw9NDOXxPlTnXx8sQ=";
            const int RETURNED_PAYMENT_ID = 1;
            const string CARD_NUMBER = "4111111111111111";
            const string CVV = "123";
            const string CURRENCY = "GBP";
            const string OWNER = "owner";
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
                Expiry = EXPIRY,
                Owner = OWNER
            };

            var acqBankMock = new Mock<IAcquiringBank>();
            acqBankMock
                .Setup(mock => mock.SendPayment(It.IsAny<AcquiringBankRequest>()))
                .ReturnsAsync(new AcquiringBankResponse
                {
                    Success = true,
                    PaymentId = RETURNED_PAYMENT_ID
                });

            var memoryCacheMock = new Mock<IMemoryCache>();
            var cacheEntryMock = new Mock<ICacheEntry>();

            memoryCacheMock
                .Setup(mock => mock.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);

            memoryCacheMock
                .Setup(mock => mock.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntryMock.Object);

            using var context = Setup.CreateContext();

            var handler = new ProcessPaymentHandler(
                acqBankMock.Object,
                nowProvider,
                memoryCacheMock.Object,
                _logger,
                _cachingOptions,
                context);
            var result = await handler.Handle(request, CancellationToken.None);

            memoryCacheMock.Verify(
                mock => mock.CreateEntry(GENERATED_REQUEST_KEY), 
                Times.Once);
        }
    }
}
