using CheckoutPaymentAPI.Models.DTOs;
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
        public Task<GetPaymentDetailsResponseDTO> Handle(GetPaymentDetailsRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
