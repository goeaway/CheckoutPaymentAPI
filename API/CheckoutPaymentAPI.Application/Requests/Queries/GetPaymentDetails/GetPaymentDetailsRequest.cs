using CheckoutPaymentAPI.Models;
using CheckoutPaymentAPI.Models.DTOs;
using MediatR;

namespace CheckoutPaymentAPI.Application.Requests.Queries.GetPaymentDetails
{
    /// <summary>
    /// Represents a request for a previously made payment
    /// </summary>
    public class GetPaymentDetailsRequest : IRequest<Either<GetPaymentDetailsResponseDTO, ErrorResponseDTO>>
    {
        /// <summary>
        /// Gets or sets the payment id of the processed payment.
        /// </summary>
        public int PaymentId { get; set; }
        /// <summary>
        /// Gets or sets the Merchant that made the payment in the first place
        /// </summary>
        public string Owner { get; set; }
    }
}