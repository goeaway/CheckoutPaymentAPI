using CheckoutPaymentAPI.Core.Abstractions;
using CheckoutPaymentAPI.Persistence;
using CheckoutPaymentAPI.Requests.Commands.ProcessPayment;
using CheckoutPaymentAPI.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Tests.Requests.Commands.ProcessPayment
{
    [TestClass]
    [TestCategory("Requests - Commands - ProcessPayment - Handler")]
    public class ProcessPaymentHandlerTests
    {
        private static (IAcquiringBank acqBank, CheckoutPaymentAPIContext context) CreateDeps()
        {
            return (
                null,
                Setup.CreateContext()
            );
        }

        [TestMethod]
        public async Task Stores_Masked_Card_Details()
        {
            var testNow = new DateTime(2021, 01, 01);

            var request = new ProcessPaymentRequest
            {
                CardNumber = "4111111111111111",
                Amount = 1,
                Currency = "GBP",
                CVV = "123",
                Expiry = testNow.AddYears(1)
            };

            var (acqBank, context) = CreateDeps();

            using (context)
            {
                var handler = new ProcessPaymentHandler(acqBank, context);
                var result = await handler.Handle(request, CancellationToken.None);

                // assert that a record is in the db for this payment id,
                // and that the card number and CVV are masked?
            }
        }

        [TestMethod]
        public async Task Returns_Payment_Id_And_Success()
        {
            var testNow = new DateTime(2021, 01, 01);

            var request = new ProcessPaymentRequest
            {
                CardNumber = "4111111111111111",
                Amount = 1,
                Currency = "GBP",
                CVV = "123",
                Expiry = testNow.AddYears(1)
            };

            var (acqBank, context) = CreateDeps();

            using (context)
            {
                var handler = new ProcessPaymentHandler(acqBank, context);
                var result = await handler.Handle(request, CancellationToken.None);

                Assert.AreEqual(1, result.PaymentId);
                Assert.IsTrue(result.Success);
            }

        }

        [TestMethod]
        public async Task Error_In_Acquiring_Bank_Returns_Unsuccessful_Response()
        {
            var testNow = new DateTime(2021, 01, 01);

            var request = new ProcessPaymentRequest
            {
                CardNumber = "4111111111111111",
                Amount = 1,
                Currency = "GBP",
                CVV = "123",
                Expiry = testNow.AddYears(1)
            };

            var (acqBank, context) = CreateDeps();

            using (context)
            {
                // use mock of acqbank to ensure we get a failure here
                var handler = new ProcessPaymentHandler(acqBank, context);
                var result = await handler.Handle(request, CancellationToken.None);

                Assert.IsNull(result.PaymentId);
                Assert.IsFalse(result.Success);
            }
        }

        [TestMethod]
        public async Task Multiple_Same_Requests_In_Time_Period_Unsuccessful()
        {
            // talk about arbitrary choice on time
            var testNow = new DateTime(2021, 01, 01);

            var request = new ProcessPaymentRequest
            {
                CardNumber = "4111111111111111",
                Amount = 1,
                Currency = "GBP",
                CVV = "123",
                Expiry = testNow.AddYears(1)
            };

            var (acqBank, context) = CreateDeps();

            using (context)
            {
                var handler = new ProcessPaymentHandler(acqBank, context);
                var result = await handler.Handle(request, CancellationToken.None);
                Assert.IsTrue(result.Success);
                // update time to be just below the limit
                // hit the handler again with same request
                var result2 = await handler.Handle(request, CancellationToken.None);

                Assert.IsFalse(result2.Success);
            }
        }

        [TestMethod]
        public async Task Multiple_Same_Requests_Outside_Time_Period_Success()
        {
            // talk about arbitrary choice on time
            var testNow = new DateTime(2021, 01, 01);

            var request = new ProcessPaymentRequest
            {
                CardNumber = "4111111111111111",
                Amount = 1,
                Currency = "GBP",
                CVV = "123",
                Expiry = testNow.AddYears(1)
            };

            var (acqBank, context) = CreateDeps();

            using (context)
            {
                var handler = new ProcessPaymentHandler(acqBank, context);
                var result = await handler.Handle(request, CancellationToken.None);

                Assert.IsTrue(result.Success);
                // update time to be just above the limit
                // hit the handler again with same request
                var result2 = await handler.Handle(request, CancellationToken.None);

                Assert.IsTrue(result2.Success);
            }
        }
    }
}
