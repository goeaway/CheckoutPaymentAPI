﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Core.Models
{
    public class AcquiringBankResponse
    {
        public bool Success { get; set; }
        public int PaymentId { get; set; }
    }
}
