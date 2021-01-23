using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Authentication
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        public IReadOnlyCollection<ApiKey> GetApiKeys()
        {
            return new List<ApiKey>
            {
                new ApiKey
                {
                    Key = "CheckoutPaymentAPI-Q2hlY2tvdXRQYXltZW50QVBJ",
                    Owner = "CheckoutPaymentAPIClient"
                },
                new ApiKey
                {
                    Key = "CheckoutPaymentAPI-DemoKey",
                    Owner = "Demo User"
                }
            };
        }
    }
}
