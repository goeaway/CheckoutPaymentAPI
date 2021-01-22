using System.Threading.Tasks;
using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Requests.Commands.ProcessPayment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CheckoutPaymentAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("process")]
        public Task<ProcessPaymentResponseDTO> Process(ProcessPaymentsRequestDTO dto)
        {
            return _mediator.Send(new ProcessPaymentRequest
            {
                CardNumber = dto.CardNumber,
                Amount = dto.Amount,
                Currency = dto.Currency,
                CVV = dto.CVV,
                Expiry = dto.Expiry,
                Owner = Request.HttpContext.GetOwnerIdentifier()
            });
        }
    }
}
