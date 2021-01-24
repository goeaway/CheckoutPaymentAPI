using CheckoutPaymentAPI.Application.AcquiringBank;
using CheckoutPaymentAPI.Core.Providers;
using CheckoutPaymentAPI.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace CheckoutPaymentAPI.Tests.Core
{
    public static class Setup
    {
        public static CheckoutPaymentAPIContext CreateContext(string databaseName = null)
        {
            var options = new DbContextOptionsBuilder<CheckoutPaymentAPIContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;
            return new CheckoutPaymentAPIContext(options);
        }

        public class CreateServerOptions
        {
            public DateTime? TestNow { get; set; }
            public IAcquiringBank AcquiringBank { get; set; }
        }

        public static (TestServer, HttpClient, CheckoutPaymentAPIContext) CreateServer(CreateServerOptions options = null)
        {
            if (options == null)
            {
                options = new CreateServerOptions();
            }

            var context = CreateContext();

            var server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton(context);

                    if (options.TestNow.HasValue)
                    {
                        services.AddSingleton(new NowProvider(options.TestNow.Value));
                    }

                    if(options.AcquiringBank != null)
                    {
                        services.AddSingleton(options.AcquiringBank);
                    }
                }));
            var client = server.CreateClient();
            return (server, client, context);
        }
    }
}
