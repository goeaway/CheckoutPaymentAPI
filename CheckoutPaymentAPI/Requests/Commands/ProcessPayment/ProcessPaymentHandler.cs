using CheckoutPaymentAPI.Core.Abstractions;
using CheckoutPaymentAPI.Exceptions;
using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Persistence;
using CheckoutPaymentAPI.Persistence.Models;
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
        private readonly INowProvider _nowProvider;

        public ProcessPaymentHandler(IAcquiringBank acquiringBank, INowProvider nowProvider, CheckoutPaymentAPIContext context)
        {
            _acquiringBank = acquiringBank;
            _context = context;
            _nowProvider = nowProvider;
        }

        public async Task<ProcessPaymentResponseDTO> Handle(ProcessPaymentRequest request, CancellationToken cancellationToken)
        {
            // mask the card number and cvv
            // use the acqbank to send payment
            var bankResponse = await _acquiringBank.SendPayment();
            // save response to newPayment
            // save newPayment to db
            var newPayment = new ProcessedPayment
            {
                Id = bankResponse.PaymentId,
                PaymentResult = bankResponse.Success,
                // mask all but last 4 digits with *
                CardNumber = new string('*', request.CardNumber.Length - 4) + request.CardNumber.Substring(request.CardNumber.Length - 4),
                // mask full length with asterisks
                CVV = new string('*', request.CVV.Length), 
                Expiry = request.Expiry,
                Amount = request.Amount,
                Created = _nowProvider.Now,
                Currency = request.Currency
            };

            _context.ProcessedPayments.Add(newPayment);

            await _context.SaveChangesAsync();

            // return response dto with new payment id and result
            return new ProcessPaymentResponseDTO
            {
                PaymentId = newPayment.Id,
                Success = newPayment.PaymentResult
            };
        }
    }
}
