using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddLogger(this IServiceCollection collection)
        {
            var logger = new LoggerConfiguration()
                .WriteTo
                    .RollingFile(Path.Combine(AppContext.BaseDirectory, "log-{Date}.log"))
                    .CreateLogger();

            collection.AddSingleton<ILogger>(logger);
            return collection;
        }
    }
}
