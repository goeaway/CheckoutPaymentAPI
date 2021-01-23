using System.Threading.Tasks;
using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Requests.Queries.GetPaymentDetails;
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
    public class PaymentDetailsController : Controller
    {
        private readonly IMediator _mediator;

        public PaymentDetailsController(IMediator mediator)
        {
            _mediator = mediator;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
        public Task<GetPaymentDetailsResponseDTO> GetPaymentDetails(int paymentId)
        {
            return _mediator.Send(
                new GetPaymentDetailsRequest 
                { 
                    PaymentId = paymentId, 
                    Owner = Request.HttpContext.GetOwnerIdentifier() 
                });
        }
    }
}
