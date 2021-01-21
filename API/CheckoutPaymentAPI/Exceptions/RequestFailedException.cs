using CheckoutPaymentAPI.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Exceptions
{
    public class RequestFailedException : CheckoutPaymentAPIException
    {
        public HttpStatusCode Code { get; set; }
            = HttpStatusCode.BadRequest;

        public RequestFailedException()
        {
        }

        public RequestFailedException(HttpStatusCode code)
        {
            Code = code;
        }

        public RequestFailedException(string message) : base(message)
        {
        }

        public RequestFailedException(string message, HttpStatusCode code) : base(message)
        {
            Code = code;
        }


        public RequestFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RequestFailedException(string message, HttpStatusCode code, Exception innerException) : base(message, innerException)
        {
            Code = code;
        }
    }
}
