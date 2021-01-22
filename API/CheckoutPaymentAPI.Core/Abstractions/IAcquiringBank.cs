using CheckoutPaymentAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Core.Abstractions
{
    public interface IAcquiringBank
    {
        Task<AcquiringBankResponse> SendPayment(AcquiringBankRequest request);
    }
}
