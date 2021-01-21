using CheckoutPaymentAPI.Models.DTOs;
using MediatR;

namespace CheckoutPaymentAPI.Requests.Queries.GetPaymentDetails
{
    public class GetPaymentDetailsRequest : IRequest<GetPaymentDetailsResponseDTO>
    {
        public int PaymentId { get; set; }
        public string Owner { get; set; }
    }
}