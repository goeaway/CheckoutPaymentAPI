using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
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
            Assert.Fail();
        }

        [TestMethod]
        public async Task Returns_Masked_Details_In_DTO()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task Returns_Bank_Payment_Result_In_DTO()
        {
            Assert.Fail();
        }
    }
}
