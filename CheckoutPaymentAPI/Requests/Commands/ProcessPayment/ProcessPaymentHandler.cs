using CheckoutPaymentAPI.Core.Abstractions;
using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Persistence;
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
        private readonly IAcquiringBank _acquiringBank;
        private readonly CheckoutPaymentAPIContext _context;

        public ProcessPaymentHandler(IAcquiringBank acquiringBank, CheckoutPaymentAPIContext context)
        {
            _acquiringBank = acquiringBank;
            _context = context;
        }

        public Task<ProcessPaymentResponseDTO> Handle(ProcessPaymentRequest request, CancellationToken cancellationToken)
        {
            // check if an existing record for this exact request has come in in the last few mins
            // make use of acqbank
            // if successful, save info from there to context
            // return response
            // otherwise just return response
            throw new NotImplementedException();
        }
    }
}
