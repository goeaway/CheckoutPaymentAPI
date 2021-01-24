using CheckoutPaymentAPI.Client.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Client
{
    /// <summary>
    /// Process payments and get payment details
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// Process a new payment
        /// </summary>
        /// <param name="cardNumber">The card number to be used in the payment</param>
        /// <param name="expiry">The expiry of the card</param>
        /// <param name="amount">The amount the payment is for</param>
        /// <param name="currency">The ISO 4217 currency code of the currency the payment is made in</param>
        /// <param name="cvv">The CVV security code of the card</param>
        /// <returns></returns>
        Task<ApiResponse<ProcessPaymentResponse>> ProcessPayment(
            string cardNumber, 
            string cvv,
            DateTime expiry,
            decimal amount,
            string currency
        );

        /// <summary>
        /// Get payment details of a previously processed payment
        /// </summary>
        /// <param name="paymentId">The id of the payment</param>
        /// <returns></returns>
        Task<ApiResponse<GetPaymentDetailsResponse>> GetPaymentDetails(int paymentId);
    }
}
