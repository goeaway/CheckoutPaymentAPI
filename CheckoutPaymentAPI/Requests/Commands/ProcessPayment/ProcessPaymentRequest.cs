using CheckoutPaymentAPI.Models.DTOs;
using MediatR;

namespace CheckoutPaymentAPI.Requests.Commands.ProcessPayment
{
    public class ProcessPaymentRequest : IRequest<ProcessPaymentResponseDTO>
    {
    }
}