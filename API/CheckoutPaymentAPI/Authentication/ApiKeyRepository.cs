using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Authentication
{
    /// <summary>
    /// Allows for retrieval of API keys
    /// </summary>
    public class ApiKeyRepository : IApiKeyRepository
    {
        /// <summary>
        /// Gets an <see cref="IReadOnlyCollection{ApiKey}"/> of API keys for the API
        /// </summary>
        /// <returns></returns>
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
