using CheckoutPaymentAPI.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Tests.Core
{
    public class ResettableNowProvider : INowProvider
    {
        private DateTime? _now;
        public DateTime Now => _now ?? DateTime.Now;

        public ResettableNowProvider(DateTime now)
        {
            _now = now;
        }

        public void Reset(DateTime now)
        {
            _now = now;
        }
    }
}
