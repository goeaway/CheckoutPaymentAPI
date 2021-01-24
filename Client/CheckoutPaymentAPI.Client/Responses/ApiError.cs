using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Client
{
    public class ApiError
    {
        public string Message { get; set; }
        public IReadOnlyCollection<string> Errors { get; set; }
    }
}
