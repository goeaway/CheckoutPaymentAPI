using CheckoutPaymentAPI.Core.Abstractions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Requests.Commands.ProcessPayment
{
    public class ProcessPaymentValidator : AbstractValidator<ProcessPaymentRequest>
    {
        public ProcessPaymentValidator(INowProvider nowProvider)
        {
            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("Card number required")
                .CreditCard().WithMessage("Card number invalid");

            RuleFor(x => x.Expiry)
                .NotEmpty().WithMessage("Expiry required")
                .GreaterThan(nowProvider.Now).WithMessage("Expiry invalid");

            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount required");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency required");

            RuleFor(x => x.CVV)
                .NotEmpty().WithMessage("CVV required")
                .Length(3).WithMessage("CVV invalid");
        }
    }
}
