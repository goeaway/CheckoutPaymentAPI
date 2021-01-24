using FluentValidation.Results;
using System;
using System.Collections.Generic;

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
