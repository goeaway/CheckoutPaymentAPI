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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentDetailsController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }
        
        [HttpGet("{identifier}")]
        public Task<GetPaymentDetailsResponseDTO> GetPaymentDetails(int identifier)
        {
            return _mediator.Send(
                new GetPaymentDetailsRequest 
                { 
                    PaymentId = identifier, 
                    Owner = _httpContextAccessor.GetOwnerIdentifier() 
                });
        }
    }
}
