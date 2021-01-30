using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Persistence;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using System.Security.Cryptography;
using CheckoutPaymentAPI.Models.AcquiringBank;
using CheckoutPaymentAPI.Application.AcquiringBank;
using CheckoutPaymentAPI.Core.Providers;
using CheckoutPaymentAPI.Application.Options;
using CheckoutPaymentAPI.Models.Entities;
using CheckoutPaymentAPI.Models;

namespace CheckoutPaymentAPI.Application.Requests.Commands.ProcessPayment
{
    /// <summary>
    /// Handles processing of a new payment sent to the API
    /// </summary>
    public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentRequest, Either<ProcessPaymentResponseDTO, ErrorResponseDTO>>
    {
        private readonly IAcquiringBank _acquiringBank;
        private readonly INowProvider _nowProvider;
        private readonly IMemoryCache _cache;
        private readonly CheckoutPaymentAPIContext _context;
        private readonly CachingOptions _cachingOptions;
        private readonly ILogger _logger;

        public ProcessPaymentHandler(
            IAcquiringBank acquiringBank, 
            INowProvider nowProvider,
            IMemoryCache cache,
            ILogger logger,
            CachingOptions cachingOptions,
            CheckoutPaymentAPIContext context)
        {
            _acquiringBank = acquiringBank;
            _nowProvider = nowProvider;
            _cache = cache;
            _logger = logger;
            _cachingOptions = cachingOptions;
            _context = context;
        }

        public async Task<Either<ProcessPaymentResponseDTO, ErrorResponseDTO>> Handle(ProcessPaymentRequest request, CancellationToken cancellationToken)
        {
            // build a hashed key based on the values of the request
            var requestCacheKey = BuildCacheKey(request);

            // check if the cache already has this key, if so throw exception
            if(_cache.TryGetValue(requestCacheKey, out _))
            {
                return new ErrorResponseDTO
                {
                    Message = "Multiple same requests detected",
                    StatusCode = HttpStatusCode.TooManyRequests
                };
            }

            _logger.Information("Verifying payment with acquiring bank");
            // use the acqbank to send payment
            var bankResponse = await _acquiringBank.SendPayment(new AcquiringBankRequest 
            {
                CardNumber = request.CardNumber,
                Amount = request.Amount,
                Currency = request.Currency,
                CVV = request.CVV,
                Expiry = request.Expiry
            });
            
            // save response to newPayment
            var newPayment = new ProcessedPayment
            {
                Id = bankResponse.PaymentId,
                PaymentResult = bankResponse.Status.ToString().Replace("_", " "),
                // mask all but last 4 digits with *
                CardNumber = new string('*', request.CardNumber.Length - 4) + request.CardNumber.Substring(request.CardNumber.Length - 4),
                // mask full length with asterisks
                CVV = new string('*', request.CVV.Length), 
                Expiry = new DateTime(request.Expiry.Year, request.Expiry.Month + 1, 1).AddDays(-1),
                Amount = request.Amount,
                Created = _nowProvider.Now,
                Currency = request.Currency,
                Owner = request.Owner
            };

            _logger.Information("Storing payment record with id {Id} for client {Owner}", newPayment.Id, newPayment.Owner);
            // add new payment to DB
            _context.ProcessedPayments.Add(newPayment);
            await _context.SaveChangesAsync();

            // add to cache, with TTL so it eventually is removed
            // using this instead of .Set() so we can mock it easier
            SetCacheForRequest(
                requestCacheKey,
                DateTimeOffset.UtcNow.AddMinutes(_cachingOptions.ProccessedPaymentTTLMinutes));

            // return response dto with new payment id and result
            return new ProcessPaymentResponseDTO
            {
                PaymentId = newPayment.Id,
                Success = bankResponse.Status == AcquiringBankResponseStatus.Success
            };
        }

        private void SetCacheForRequest(string key, DateTimeOffset offset)
        {
            using var entry = _cache.CreateEntry(key);

            entry.Value = 1;
            entry.AbsoluteExpiration = offset;
        }

        private string BuildCacheKey(ProcessPaymentRequest request)
        {
            var builder = new StringBuilder();
            // combine all parts of the request into a string
            builder.Append(request.CardNumber);
            builder.Append(request.Amount + "");
            builder.Append(request.Currency);
            builder.Append(request.CVV);
            builder.Append(request.Expiry.Month + request.Expiry.Year + "");
            builder.Append(request.Owner);

            using var sha256 = SHA256.Create();
            // hash the combined strings to protect them
            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            // convert to base 64
            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }
    }
}
