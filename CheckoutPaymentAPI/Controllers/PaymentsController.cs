using System.Threading.Tasks;
using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Requests.Commands.ProcessPayment;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CheckoutPaymentAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;

        public PaymentsController(IMediator mediator, IMemoryCache cache)
        {
            _mediator = mediator;
            _cache = cache;
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
                Expiry = dto.Expiry
            });
        }
    }
}
