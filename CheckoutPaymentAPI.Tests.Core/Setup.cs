using CheckoutPaymentAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Tests.Core
{
    public static class Setup
    {
        public static CheckoutPaymentAPIContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<CheckoutPaymentAPIContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new CheckoutPaymentAPIContext(options);
        }
    }
}
