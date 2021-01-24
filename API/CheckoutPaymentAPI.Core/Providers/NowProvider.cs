using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Core.Providers
{
    /// <summary>
    /// Provides easier way to mock DateTime.Now calls by providing settable Now
    /// </summary>
    public class NowProvider : INowProvider
    {
        private readonly DateTime? _now;
        /// <summary>
        /// Gets a <see cref="DateTime"/> representing now
        /// </summary>
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
