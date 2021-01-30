using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Models.AcquiringBank
{
    public enum AcquiringBankResponseStatus
    {
        Success,
        Payment_Declined,
        Acquiring_Bank_Unreachable
    }
}
