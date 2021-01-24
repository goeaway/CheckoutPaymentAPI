using CheckoutPaymentAPI.Application.Options;
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
    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/> objects
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adds a logging service to the collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IServiceCollection AddLogger(this IServiceCollection collection)
        {
            var logger = new LoggerConfiguration()
                .WriteTo
                    .RollingFile(Path.Combine(AppContext.BaseDirectory, "log-{Date}.log"))
                    .CreateLogger();

            collection.AddSingleton<ILogger>(logger);
            return collection;
        }

        /// <summary>
        /// Adds bound caching options found in the <see cref="IConfiguration"/>
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCachingOptions(this IServiceCollection collection, IConfiguration configuration)
        {
            var options = new CachingOptions();
            configuration.GetSection(CachingOptions.Key).Bind(options);
            return collection.AddSingleton(options);
        }
    }
}
