using CheckoutPaymentAPI.Client.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Client
{
    public interface IAPIClient
    {
        Task<ClientResponse<ProcessPaymentResponse>> ProcessPayment(
            string cardNumber, 
            DateTime expiry,
            decimal amount,
            string currency,
            string cvv
        );

        Task<ClientResponse<GetPaymentDetailsResponse>> GetPaymentDetails(int paymentId);
    }
}
