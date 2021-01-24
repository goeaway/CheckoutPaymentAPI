using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Providers
{
    public class NowProvider : INowProvider
    {
        private readonly DateTime? _now;
        public DateTime Now => _now ?? DateTime.Now;

        public NowProvider()
        {

        }

        public NowProvider(DateTime now)
        {
            _now = now;
        }
    }
}
