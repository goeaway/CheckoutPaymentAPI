using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Client.Responses
{
    public class ProcessPaymentResponse
    {
        public bool Success { get; set; }
        public int PaymentId { get; set; }
    }
}
