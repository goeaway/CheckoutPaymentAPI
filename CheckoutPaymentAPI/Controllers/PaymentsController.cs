using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Requests.Commands.ProcessPayment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutPaymentAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public Task Process(ProcessPaymentsRequestDTO dto)
        {
            return _mediator.Send(new ProcessPaymentRequest());
        }
    }
}
