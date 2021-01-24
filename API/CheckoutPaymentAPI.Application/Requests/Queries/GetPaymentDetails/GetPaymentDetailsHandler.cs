using CheckoutPaymentAPI.Application.Exceptions;
using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Persistence;
using MediatR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Application.Requests.Queries.GetPaymentDetails
{
    /// <summary>
    /// Handles the retrieval of a previously processed payment
    /// </summary>
    public class GetPaymentDetailsHandler : IRequestHandler<GetPaymentDetailsRequest, GetPaymentDetailsResponseDTO>
    {
        private readonly ILogger _logger;
        private readonly CheckoutPaymentAPIContext _context;

        public GetPaymentDetailsHandler(ILogger logger, CheckoutPaymentAPIContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<GetPaymentDetailsResponseDTO> Handle(GetPaymentDetailsRequest request, CancellationToken cancellationToken)
        {
            // try and find payment in context
            var foundPayment = await _context.ProcessedPayments.FindAsync(request.PaymentId);

            // throw error with message if not found or if not the right owner, will be picked up by error handler in startup
            if(foundPayment == null || foundPayment.Owner != request.Owner)
            {
                throw new RequestFailedException($"Payment details not found", HttpStatusCode.NotFound);
            }

            _logger.Information("Retrieving found payment with id {Id}", foundPayment.Id);
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
