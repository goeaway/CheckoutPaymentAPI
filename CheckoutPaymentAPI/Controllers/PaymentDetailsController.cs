using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Requests.Queries.GetPaymentDetails;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutPaymentAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentDetailsController : Controller
    {
        private readonly IMediator _mediator;

        public PaymentDetailsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        public Task<GetPaymentDetailsResponseDTO> GetPaymentDetails(string identifier)
        {
            return _mediator.Send(new GetPaymentDetailsRequest());
        }
    }
}
