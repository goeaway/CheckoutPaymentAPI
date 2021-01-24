using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Authentication
{
    /// <summary>
    /// Allows for retrieval of API keys
    /// </summary>
    public interface IApiKeyRepository
    {
        /// <summary>
        /// Gets an <see cref="IReadOnlyCollection{ApiKey}"/> of API keys for the API
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<ApiKey> GetApiKeys();
    }
}
