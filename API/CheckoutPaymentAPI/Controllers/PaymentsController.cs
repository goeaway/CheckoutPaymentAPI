using System.Threading.Tasks;
using CheckoutPaymentAPI.Application.Requests.Commands.ProcessPayment;
using CheckoutPaymentAPI.Models.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutPaymentAPI.Controllers
{
    [Produces("application/json")]
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

        /// <summary>
        /// Processes a payment
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <response code="200">Returns whether the process was a success and a payment id to associate with this process</response>
        /// <response code="400">If the request body contains one or more validation errors</response>
        /// <response code="401">If the request is not authenticated</response>
        [HttpPost("process")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
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
