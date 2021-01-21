using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Requests.Queries.GetPaymentDetails
{
    public class GetPaymentDetailsValidator : AbstractValidator<GetPaymentDetailsRequest>
    {
        public GetPaymentDetailsValidator()
        {
            RuleFor(x => x.PaymentId).NotEmpty().WithMessage("Payment id required");
        }
    }
}
