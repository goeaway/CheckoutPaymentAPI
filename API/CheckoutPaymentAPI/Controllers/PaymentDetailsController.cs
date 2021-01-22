using System.Threading.Tasks;
using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Requests.Queries.GetPaymentDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutPaymentAPI.Controllers
{
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
        
        [HttpGet("{identifier}")]
        public Task<GetPaymentDetailsResponseDTO> GetPaymentDetails(int identifier)
        {
            return _mediator.Send(
                new GetPaymentDetailsRequest 
                { 
                    PaymentId = identifier, 
                    Owner = Request.HttpContext.GetOwnerIdentifier() 
                });
        }
    }
}
