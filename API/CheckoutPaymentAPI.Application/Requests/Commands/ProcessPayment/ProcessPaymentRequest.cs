using CheckoutPaymentAPI.Models.DTOs;
using MediatR;
using System;

namespace CheckoutPaymentAPI.Application.Requests.Commands.ProcessPayment
{
    /// <summary>
    /// Represents a new payment to be processed
    /// </summary>
    public class ProcessPaymentRequest : IRequest<ProcessPaymentResponseDTO>
    {
        /// <summary>
        /// Gets or sets the card long number used in the payment
        /// </summary>
        public string CardNumber { get; set; }
        /// <summary>
        /// Gets or sets the expiry date of the card used in the payment
        /// </summary>
        public DateTime Expiry { get; set; }
        /// <summary>
        /// Gets or sets the decimal amount of this payment
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Gets or sets the ISO 4217 currency code string this payment was made in
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Gets or sets the CVV security code of the card used in the payment
        /// </summary>
        public string CVV { get; set; }
        /// <summary>
        /// Gets or sets the Merchant that is making this request
        /// </summary>
        public string Owner { get; set; }
    }
}