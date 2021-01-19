using CheckoutPaymentAPI.Exceptions;
using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Requests.Queries.GetPaymentDetails
{
    public class GetPaymentDetailsHandler : IRequestHandler<GetPaymentDetailsRequest, GetPaymentDetailsResponseDTO>
    {
        private readonly CheckoutPaymentAPIContext _context;

        public GetPaymentDetailsHandler(CheckoutPaymentAPIContext context)
        {
            _context = context;
        }
        public async Task<GetPaymentDetailsResponseDTO> Handle(GetPaymentDetailsRequest request, CancellationToken cancellationToken)
        {
            // try and find payment in context
            var foundPayment = await _context.ProcessedPayments.FindAsync(request.PaymentId);

            // throw error with message if not found, will be picked up by error handler in startup
            if(foundPayment == null)
            {
                throw new RequestFailedException($"No payment details could be found for id {request.PaymentId}");
            }

            // could use Automapper here
            return new GetPaymentDetailsResponseDTO 
            { 
                CardNumber = foundPayment.CardNumber,
                Expiry = foundPayment.Expiry,
                Currency = foundPayment.Currency,
                Amount = foundPayment.Amount,
                CVV = foundPayment.CVV,
                PaymentResult = foundPayment.PaymentResult
            };
        }
    }
}
