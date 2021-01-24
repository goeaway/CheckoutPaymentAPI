using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Core.Providers
{
    /// <summary>
    /// Provides easier way to mock DateTime.Now calls by providing settable Now
    /// </summary>
    public interface INowProvider
    {
        /// <summary>
        /// Gets a <see cref="DateTime"/> representing now
        /// </summary>
        public DateTime Now { get; }
    }
}
