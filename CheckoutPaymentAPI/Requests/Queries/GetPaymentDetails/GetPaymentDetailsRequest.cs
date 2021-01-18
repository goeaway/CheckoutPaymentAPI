using CheckoutPaymentAPI.Models.DTOs;
using MediatR;

namespace CheckoutPaymentAPI.Requests.Queries.GetPaymentDetails
{
    public class GetPaymentDetailsRequest : IRequest<GetPaymentDetailsResponseDTO>
    {
    }
}