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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentsController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
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
                Owner = _httpContextAccessor.GetOwnerIdentifier()
            });
        }
    }
}
