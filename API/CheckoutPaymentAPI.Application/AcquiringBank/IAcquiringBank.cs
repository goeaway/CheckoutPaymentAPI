using CheckoutPaymentAPI.Models;
using CheckoutPaymentAPI.Models.AcquiringBank;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Application.AcquiringBank
{
    /// <summary>
    /// Sends payments to a bank service.
    /// </summary>
    public interface IAcquiringBank
    {
        /// <summary>
        /// Sends a payment request to a bank service and returns a response indicating whether successful.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<AcquiringBankResponse> SendPayment(AcquiringBankRequest request);
    }
}
