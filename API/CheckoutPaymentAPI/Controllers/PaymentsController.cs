using System.Threading.Tasks;
using CheckoutPaymentAPI.Application.Requests.Commands.ProcessPayment;
using CheckoutPaymentAPI.Application.Requests.Queries.GetPaymentDetails;
using CheckoutPaymentAPI.Models;
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
        /// <response code="429">If the request is the same as a recently made request</response>
        [ProducesResponseType(typeof(ProcessPaymentResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status429TooManyRequests)]
        [HttpPost]
        public async Task<IActionResult> Process(ProcessPaymentsRequestDTO dto)
        {
            var result = await _mediator.Send(new ProcessPaymentRequest
            {
                CardNumber = dto.CardNumber,
                Amount = dto.Amount,
                Currency = dto.Currency,
                CVV = dto.CVV,
                Expiry = dto.Expiry,
                Owner = Request.HttpContext.GetOwnerIdentifier()
            });

            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.StatusCode, error)
            );
        }

        /// <summary>
        /// Retrieves information of a previously processed payment
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        /// <response code="200">Returns payment details associated with the provided id</response>
        /// <response code="404">If the payment details could not be found</response>
        /// <response code="401">If the request is not authenticated</response>
        [HttpGet("{paymentId}")]
        [ProducesResponseType(typeof(GetPaymentDetailsResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPaymentDetails(int paymentId)
        {
            var result = await _mediator.Send(
                new GetPaymentDetailsRequest
                {
                    PaymentId = paymentId,
                    Owner = Request.HttpContext.GetOwnerIdentifier()
                });

            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.StatusCode, error)
            );
        }
    }
}
