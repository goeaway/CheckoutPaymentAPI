using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI
{
    /// <summary>
    /// Provides extension methods for <see cref="HttpContext"/>
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Returns the owner identifier value from the <see cref="HttpContext"/> claims collection
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetOwnerIdentifier(this HttpContext httpContext)
        {
            return httpContext
                .User
                .Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;
        }
    }
}
