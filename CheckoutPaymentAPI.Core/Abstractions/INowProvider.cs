using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Core.Abstractions
{
    /// <summary>
    /// Provides easier way to mock DateTime.Now calls by providing settable Now
    /// </summary>
    public interface INowProvider
    {
     
        public DateTime Now { get; }
    }
}
