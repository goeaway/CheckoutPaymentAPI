using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using CheckoutPaymentAPI.Behaviours;
using CheckoutPaymentAPI.Core;
using CheckoutPaymentAPI.Core.Abstractions;
using CheckoutPaymentAPI.Core.Providers;
using CheckoutPaymentAPI.Exceptions;
using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Persistence;
using CheckoutPaymentAPI.Requests.Commands.ProcessPayment;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace CheckoutPaymentAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
                .AddApiKeyInHeader(options =>
                {
                    options.Realm = "CheckoutPaymentAPI";
                    options.KeyName = "X-API-KEY";
                    options.Events = new ApiKeyEvents
                    {
                        OnValidateKey = async (context) =>
                        {
                            // validate api key header value
                            // in production this event implemention would make use of some key store
                            // such as secret manager and not have a hardcoded key value here
                            if (context.ApiKey == "CheckoutPaymentAPI-Q2hlY2tvdXRQYXltZW50QVBJ")
                            {
                                var claims = new[]
                                {
                                    new Claim(ClaimTypes.NameIdentifier, "CheckoutPaymentAPIClient", ClaimValueTypes.String, context.Options.ClaimsIssuer),
                                    new Claim(ClaimTypes.Name, "CheckoutPaymentAPIClient", ClaimValueTypes.String, context.Options.ClaimsIssuer),
                                };

                                context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                                context.Success();
                            }
                            else
                            {
                                context.NoResult();
                            }
                        }
                    };
                });

            services.AddCachingOptions(Configuration);
            services.AddControllers().AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<ProcessPaymentValidator>();
            });

            services.AddLogger();
            services.AddMemoryCache();
            services.AddMediatR(Assembly.GetAssembly(typeof(ProcessPaymentHandler)));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient<IAcquiringBank, AcquiringBank>();

            services.AddSingleton<INowProvider>(new NowProvider());

            services.AddDbContext<CheckoutPaymentAPIContext>(
                options => options.UseInMemoryDatabase("CheckoutPaymentAPIDatabase"));

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseExceptionHandler(ExceptionHandler);

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ExceptionHandler(IApplicationBuilder app)
        {
            app.Run(async ctx =>
            {
                ctx.Response.StatusCode = 500;
                ctx.Response.ContentType = "application/json";
                var exHandlerPathFeature = ctx.Features.Get<IExceptionHandlerFeature>();
                var exception = exHandlerPathFeature.Error;
                var uri = ctx.Request.Path;

                var logger = app.ApplicationServices.GetService<ILogger>();
                logger.Error(exception, "Error occurred when processing request {uri}", uri);

                var errorResponse = new ErrorResponseDTO();

                if (exception is RequestValidationFailedException)
                {
                    ctx.Response.StatusCode = 400;
                    errorResponse.Message = "Validation error";
                    errorResponse.Errors.AddRange((exception as RequestValidationFailedException).Failures.Select(f => f.ErrorMessage));
                }
                else if (exception is RequestFailedException)
                {
                    var requestFailedException = exception as RequestFailedException;
                    ctx.Response.StatusCode = (int)requestFailedException.Code;
                    errorResponse.Message = requestFailedException.Message;
                } 
                else
                {
                    errorResponse.Message = exception.Message;
                }

                await ctx.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            });
        }
    }
}
