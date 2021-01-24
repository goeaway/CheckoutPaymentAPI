using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Models.AcquiringBank
{
    /// <summary>
    /// Respresents a response from an acq bank request
    /// </summary>
    public class AcquiringBankResponse
    {
        /// <summary>
        /// Gets or sets a value indicating if the request was a success
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Gets or sets a value identifying the payment
        /// </summary>
        public int PaymentId { get; set; }
    }
}
