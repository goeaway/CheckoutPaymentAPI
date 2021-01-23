using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Authentication
{
    public interface IApiKeyRepository
    {
        IReadOnlyCollection<ApiKey> GetApiKeys();
    }
}
