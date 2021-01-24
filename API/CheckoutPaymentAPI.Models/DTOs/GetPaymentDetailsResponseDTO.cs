using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Models.DTOs
{
    /// <summary>
    /// Represents what the API will return when responding to a get payment details request
    /// </summary>
    public class GetPaymentDetailsResponseDTO
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
        /// Gets or sets a boolean value indicating if the payment was a success
        /// </summary>
        public bool PaymentResult { get; set; }
    }
}
