using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Application.Requests.Queries.GetPaymentDetails
{
    /// <summary>
    /// Validates a get payment details request
    /// </summary>
    public class GetPaymentDetailsValidator : AbstractValidator<GetPaymentDetailsRequest>
    {
        public GetPaymentDetailsValidator()
        {
            // payment id must not be empty
            RuleFor(x => x.PaymentId).NotEmpty().WithMessage("Payment id required");
            // owner must not be empty
            RuleFor(x => x.Owner).NotEmpty().WithMessage("Owner required");
        }
    }
}
