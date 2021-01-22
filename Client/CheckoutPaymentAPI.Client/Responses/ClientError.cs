using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Client
{
    public class ClientError
    {
        public string Message { get; set; }
        public IReadOnlyCollection<string> Errors { get; set; }
    }
}
