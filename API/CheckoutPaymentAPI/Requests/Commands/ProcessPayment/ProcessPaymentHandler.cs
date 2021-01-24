﻿using CheckoutPaymentAPI.Exceptions;
using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Options;
using CheckoutPaymentAPI.Persistence;
using CheckoutPaymentAPI.Persistence.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using CheckoutPaymentAPI.Models;
using System.Security.Cryptography;
using CheckoutPaymentAPI.AcquiringBank;
using CheckoutPaymentAPI.Providers;

namespace CheckoutPaymentAPI.Requests.Commands.ProcessPayment
{
    public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentRequest, ProcessPaymentResponseDTO>
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

        public async Task<ProcessPaymentResponseDTO> Handle(ProcessPaymentRequest request, CancellationToken cancellationToken)
        {
            // build a key based on the values of the request
            var requestCacheKey = BuildCacheKey(request);

            // check if the cache already has this key, if so throw exception
            if(_cache.TryGetValue(requestCacheKey, out _))
            {
                throw new RequestFailedException("Multiple same request detected", HttpStatusCode.TooManyRequests);
            }

            // use the acqbank to send payment
            _logger.Information("Verifying payment with acquiring bank");
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
                PaymentResult = bankResponse.Success,
                // mask all but last 4 digits with *
                CardNumber = new string('*', request.CardNumber.Length - 4) + request.CardNumber.Substring(request.CardNumber.Length - 4),
                // mask full length with asterisks
                CVV = new string('*', request.CVV.Length), 
                Expiry = request.Expiry,
                Amount = request.Amount,
                Created = _nowProvider.Now,
                Currency = request.Currency,
                Owner = request.Owner
            };

            // add new payment to DB
            _logger.Information("Storing payment record with id {Id} for client {Owner}", newPayment.Id, newPayment.Owner);
            _context.ProcessedPayments.Add(newPayment);
            await _context.SaveChangesAsync();

            // add to cache, with TTL so it eventually is removed
            // use this instead of .Set() so we can mock it easier
            SetCacheForRequest(
                requestCacheKey,
                DateTimeOffset.UtcNow.AddMinutes(_cachingOptions.ProccessedPaymentTTLMinutes));

            // return response dto with new payment id and result
            return new ProcessPaymentResponseDTO
            {
                PaymentId = newPayment.Id,
                Success = newPayment.PaymentResult
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

            builder.Append(request.CardNumber);
            builder.Append(request.Amount + "");
            builder.Append(request.Currency);
            builder.Append(request.CVV);
            builder.Append(request.Expiry.ToShortDateString());
            builder.Append(request.Owner);

            using var sha256 = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }
    }
}
