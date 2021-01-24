using CheckoutPaymentAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.AcquiringBank
{
    public class AcquiringBank : IAcquiringBank
    {
        public Task<AcquiringBankResponse> SendPayment(AcquiringBankRequest request)
        {
            // faked result
            return Task.Run(() => new AcquiringBankResponse
            {
                Success = true,
                PaymentId = new Random().Next()
            });
        }
    }
}
