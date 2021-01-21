using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Persistence.Models
{
    public class ProcessedPayment
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public DateTime Expiry { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public bool PaymentResult { get; set; }
        public DateTime Created { get; set; }
        public string Owner { get; set; }
    }
}
