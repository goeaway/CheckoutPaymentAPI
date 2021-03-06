using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using CheckoutPaymentAPI.Application.AcquiringBank;
using CheckoutPaymentAPI.Application.Behaviours;
using CheckoutPaymentAPI.Application.Requests.Commands.ProcessPayment;
using CheckoutPaymentAPI.Authentication;
using CheckoutPaymentAPI.Core.Providers;
using CheckoutPaymentAPI.Models.DTOs;
using CheckoutPaymentAPI.Persistence;
using CheckoutPaymentAPI.Swagger;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;

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
                        OnValidateKey = (context) => Task.Run(() =>
                        {
                            var keyRepo = context.HttpContext.RequestServices.GetRequiredService<IApiKeyRepository>();
                            var keys = keyRepo.GetApiKeys();

                            var match = keys.FirstOrDefault(key => key.Key == context.ApiKey);

                            if(match != null)
                            {
                                var claims = new[]
                                {
                                    new Claim(ClaimTypes.NameIdentifier, match.Owner, ClaimValueTypes.String, context.Options.ClaimsIssuer),
                                    new Claim(ClaimTypes.Name, match.Owner, ClaimValueTypes.String, context.Options.ClaimsIssuer),
                                };

                                context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                                context.Success();
                            }
                            else
                            {
                                context.NoResult();
                            }
                        })
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

            services.AddSingleton<IApiKeyRepository, ApiKeyRepository>();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Checkout Payment API",
                    Description = "A checkout payment gateaway",
                });

                c.OperationFilter<AddRequiredHeaderParameter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Checkout Payment API");
            });

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

                var errorResponse = new ErrorResponseDTO 
                {
                    Message = exception.Message
                };

                await ctx.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            });
        }
    }
}
