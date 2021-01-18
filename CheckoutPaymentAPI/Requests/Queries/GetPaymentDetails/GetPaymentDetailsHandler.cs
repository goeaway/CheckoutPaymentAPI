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
        public Task<GetPaymentDetailsResponseDTO> Handle(GetPaymentDetailsRequest request, CancellationToken cancellationToken)
        {
            // query context for details with provided id
            // throw error if not found (request failed exception probably)
            throw new NotImplementedException();
        }
    }
}
