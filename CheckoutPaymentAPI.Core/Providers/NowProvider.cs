using CheckoutPaymentAPI.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Core.Providers
{
    public class NowProvider : INowProvider
    {
        private readonly DateTime? _now;
        public DateTime Now => _now ?? DateTime.Now;
    }
}
