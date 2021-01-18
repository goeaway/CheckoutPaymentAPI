using CheckoutPaymentAPI.Requests.Queries.GetPaymentDetails;
using CheckoutPaymentAPI.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Tests.Requests.Queries.GetPaymentDetails
{
    [TestClass]
    [TestCategory("Requests - Queries - GetPaymentDetails - Handler")]
    public class GetPaymentHandlerTests
    {
        [TestMethod]
        public async Task Throws_If_No_Data_Found_For_Id()
        {
            var request = new GetPaymentDetailsRequest
            {
                PaymentId = 1
            };

            using (var context = Setup.CreateContext())
            {
                var handler = new GetPaymentDetailsHandler(context);
                var result = await handler.Handle(request, CancellationToken.None);

                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task Returns_Masked_Details_In_DTO()
        {
            var request = new GetPaymentDetailsRequest
            {
                PaymentId = 1
            };

            using (var context = Setup.CreateContext())
            {
                var handler = new GetPaymentDetailsHandler(context);
                var result = await handler.Handle(request, CancellationToken.None);

                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task Returns_Bank_Payment_Result_In_DTO()
        {
            var request = new GetPaymentDetailsRequest
            {
                PaymentId = 1
            };

            using (var context = Setup.CreateContext())
            {
                var handler = new GetPaymentDetailsHandler(context);
                var result = await handler.Handle(request, CancellationToken.None);

                Assert.Fail();
            }
        }
    }
}
