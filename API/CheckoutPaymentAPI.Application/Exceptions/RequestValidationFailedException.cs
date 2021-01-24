using CheckoutPaymentAPI.Core.Exceptions;
using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace CheckoutPaymentAPI.Application.Exceptions
{
    /// <summary>
    /// Represents one or more validation failures for a request. Includes all validation failures
    /// </summary>
    public class RequestValidationFailedException : CheckoutPaymentAPIException
    {
        /// <summary>
        /// Gets a collection of validation failures
        /// </summary>
        public IReadOnlyCollection<ValidationFailure> Failures { get; }
            = new List<ValidationFailure>();

        public RequestValidationFailedException(IReadOnlyCollection<ValidationFailure> failures)
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
