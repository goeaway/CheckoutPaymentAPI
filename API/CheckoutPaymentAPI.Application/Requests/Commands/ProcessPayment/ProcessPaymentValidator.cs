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
                .NotEmpty().WithMessage("Card number required");

            When(x => !string.IsNullOrWhiteSpace(x.CardNumber), () =>
            {
                RuleFor(x => x.CardNumber)
                    .Must(x => ValidCreditCard(x))
                    .WithMessage("Card number invalid");
            });

            // must have expiry and it must be in the future
            RuleFor(x => x.Expiry)
                .NotEmpty().WithMessage("Expiry required");

            When(x => x.Expiry != null, () =>
            {
                RuleFor(x => x.Expiry.Year)
                    .NotEmpty()
                    .WithMessage("Expiry year required");

                RuleFor(x => x.Expiry.Month)
                    .InclusiveBetween(1, 12)
                    .WithMessage("Expiry month must be between 1 and 12");

                RuleFor(x => x.Expiry)
                    .Must(x => new DateTime(x.Year, x.Month, 1).AddMonths(1).AddDays(-1) >= nowProvider.Now.Date)
                    .When(x => x.Expiry.Year > 0 && x.Expiry.Month > 0 && x.Expiry.Month < 13)
                    .WithMessage("Expiry invalid");
            });

            // must have amount and it must not be 0
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than 0");

            // must have a currency and it must be in the approved list
            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency required")
                .Must(x => SupportedCurrencies().Contains(x)).WithMessage("ISO 4217 Currency code not recognised");

            // must have cvv and it must be 3 digits long
            RuleFor(x => x.CVV)
                .NotEmpty().WithMessage("CVV required");

            When(x => !string.IsNullOrWhiteSpace(x.CVV), () =>
            {
                RuleFor(x => x.CVV)
                    .Length(3)
                    .WithMessage("CVV must be 3 characters long");

                When(x => x.CVV.Length == 3, () =>
                {
                    RuleFor(x => x.CVV)
                        .Must(x => int.TryParse(x, out _))
                        .WithMessage("CVV must not contain letters or symbols");
                });
            });

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

        private bool ValidCreditCard(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            value = value.Replace("-", "").Replace(" ", "");

            var checksum = 0;
            var evenDigit = false;

            foreach (char digit in value.ToCharArray().Reverse())
            {
                if (!char.IsDigit(digit))
                {
                    return false;
                }

                var digitValue = (digit - '0') * (evenDigit ? 2 : 1);
                evenDigit = !evenDigit;

                while (digitValue > 0)
                {
                    checksum += digitValue % 10;
                    digitValue /= 10;
                }
            }

            return (checksum % 10) == 0;
        }
    }
}
