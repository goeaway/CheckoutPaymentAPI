using CheckoutPaymentAPI.Core.Providers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckoutPaymentAPI.Application.Requests.Commands.ProcessPayment
{
    /// <summary>
    /// Validates a processed payment request
    /// </summary>
    public class ProcessPaymentValidator : AbstractValidator<ProcessPaymentRequest>
    {
        public ProcessPaymentValidator(INowProvider nowProvider)
        {
            // must have card number and it must be a valid one
            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("Card number required")
                .CreditCard().WithMessage("Card number invalid");

            // must have expiry and it must be in the future
            RuleFor(x => x.Expiry)
                .NotEmpty().WithMessage("Expiry required")
                .GreaterThan(nowProvider.Now).WithMessage("Expiry invalid");

            // must have amount and it must not be 0
            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount required");

            // must have a currency and it must be in the approved list
            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency required")
                .Must(x => SupportedCurrencies().Contains(x)).WithMessage("ISO 4217 Currency code not recognised");

            // must have cvv and it must be 3 digits long
            RuleFor(x => x.CVV)
                .NotEmpty().WithMessage("CVV required")
                .Length(3).WithMessage("CVV invalid");

            RuleFor(x => x.Owner)
                .NotEmpty().WithMessage("Owner required");
        }

        private IReadOnlyCollection<string> SupportedCurrencies()
        {
            // This is just a small sample of the ISO 4217 list
            // in a real application, the full list should be provided
            // it may also be better to store this data in a DB and then call on it at startup
            // adding a service to the DI would be a good idea
            return new List<string>
            {
                "GBP",
                "EUR",
                "USD",
                "AUD",
                "CAD",
                "JPY",
                "CNY",
                "HKD",
                "INR",
                "RUB",
                "SAR"
            };
        }
    }
}
