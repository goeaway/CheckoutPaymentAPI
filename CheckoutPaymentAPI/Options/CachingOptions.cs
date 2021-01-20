using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Options
{
    public class CachingOptions
    {
        public const string Key = "Caching";

        public int ProccessedPaymentTTLMinutes { get; set; }
            = 1;
    }
}
