using CheckoutPaymentAPI.Core.Exceptions;
using System;
using System.Net;

namespace CheckoutPaymentAPI.Application.Exceptions
{
    /// <summary>
    /// Represents a failed request and includes a settable <see cref="HttpStatusCode"/>
    /// </summary>
    public class RequestFailedException : CheckoutPaymentAPIException
    {
        /// <summary>
        /// Gets or sets an <see cref="HttpStatusCode"/> for this request
        /// </summary>
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
