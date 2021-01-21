using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI
{
    public static class HttpContextAccessorExtensions
    {
        public static string GetOwnerIdentifier(this IHttpContextAccessor contextAccessor)
        {
            return contextAccessor
                .HttpContext
                .User
                .Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;
        }
    }
}
