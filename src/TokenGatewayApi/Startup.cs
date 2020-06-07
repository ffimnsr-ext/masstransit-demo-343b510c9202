using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration; // 1
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions; // 1
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks; // 1
using MassTransit; // 1
using MassTransit.Definition; // 1
using TokenContracts; // 1

namespace TokenGatewayApi
{
    public class Startup
    {
        private IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            services.AddMassTransit(config =>
            {
                config.AddBus(context => Bus.Factory.CreateUsingRabbitMq(c => 
                {
                    c.UseHealthCheck(context);
                    c.Host(_configuration.GetValue("RabbitHost", "rabbitmq://localhost"));
                }));

                config.AddRequestClient<SubmitToken>();
            });

            services.AddMassTransitHostedService();
            services.AddControllers();
            services.AddCors();
            services.AddRouting(options => options.LowercaseUrls = true);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(policy => 
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Token Gateway Api Service");
                });

                endpoints.MapControllers();

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains("ready"),
                });

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions()
                {
                    Predicate = (_) => false
                });                
            });
        }
    }
}
