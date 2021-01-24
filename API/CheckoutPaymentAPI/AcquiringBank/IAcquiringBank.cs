using CheckoutPaymentAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.AcquiringBank
{
    public interface IAcquiringBank
    {
        Task<AcquiringBankResponse> SendPayment(AcquiringBankRequest request);
    }
}
