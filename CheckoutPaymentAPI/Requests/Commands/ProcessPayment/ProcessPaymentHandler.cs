using CheckoutPaymentAPI.Models.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Requests.Commands.ProcessPayment
{
    public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentRequest, ProcessPaymentResponseDTO>
    {

        public Task<ProcessPaymentResponseDTO> Handle(ProcessPaymentRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
