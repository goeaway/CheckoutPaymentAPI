using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CheckoutPaymentAPI.Core.Exceptions
{
    public class CheckoutPaymentAPIException : Exception
    {
        public CheckoutPaymentAPIException()
        {
        }

        public CheckoutPaymentAPIException(string message) : base(message)
        {
        }

        public CheckoutPaymentAPIException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
