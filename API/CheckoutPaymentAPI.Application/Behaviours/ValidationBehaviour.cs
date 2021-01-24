using CheckoutPaymentAPI.Application.Exceptions;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Application.Behaviours
{
    /// <summary>
    /// Mediatr pipeline behaviour to check request validations
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var ctx = new ValidationContext<TRequest>(request);
            // find all failed validations related to this request
            var failures = _validators
                .Select(v => v.Validate(ctx))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            // throw if there are failures. This exception is picked up by the app's error response handler
            if (failures.Count != 0)
            {
                throw new RequestValidationFailedException(failures);
            }

            return next();
        }
    }
}
