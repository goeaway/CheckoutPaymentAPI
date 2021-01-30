using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Models.Entities
{
    /// <summary>
    /// Represents a processed payment
    /// </summary>
    public class ProcessedPayment
    {
        /// <summary>
        /// Gets or sets a numeric identifier for the payment
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets a string representing the card number that was used in the payment
        /// </summary>
        public string CardNumber { get; set; }
        /// <summary>
        /// Gets or sets a string representing a CVV security code of the card used in the payment
        /// </summary>
        public string CVV { get; set; }
        /// <summary>
        /// Gets or sets the expiry of a card
        /// </summary>
        public DateTime Expiry { get; set; }
        /// <summary>
        /// Gets or sets the ISO 4217 currency code a payment was made in
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Gets or sets the amount the payment was for
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Gets or sets a value indicating the result of the payment
        /// </summary>
        public string PaymentResult { get; set; }
        /// <summary>
        /// Gets or sets when the payment was saved
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the merchant identifier that made the payment
        /// </summary>
        public string Owner { get; set; }
    }
}
