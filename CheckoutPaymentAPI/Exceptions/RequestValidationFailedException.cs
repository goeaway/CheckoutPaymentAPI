using CheckoutPaymentAPI.Core.Exceptions;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Exceptions
{
    public class RequestValidationFailedException : CheckoutPaymentAPIException
    {
        public IList<ValidationFailure> Failures { get; }
            = new List<ValidationFailure>();

        public RequestValidationFailedException(IList<ValidationFailure> failures)
        {
            Failures = failures;
        }

        public RequestValidationFailedException()
        {
        }

        public RequestValidationFailedException(string message) : base(message)
        {
        }

        public RequestValidationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
