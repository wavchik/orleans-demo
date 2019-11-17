using System;
using System.Net;
using Demo.Orleans.SiloHost.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Demo.SmartCache.GrainImplementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Demo.Orleans.SiloHost
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(_ =>
                {
                    var config = Configuration.GetSection("orleans");
                    var builder = new SiloHostBuilder()
                        .AddMemoryGrainStorage("RegistryStore")
                        .AddMemoryGrainStorage("RegistryItemStore")
                        .UseDashboard(options => config.GetSection("dashboard").Bind(options))
                        .Configure<ClusterOptions>(options =>
                        {
                            config.GetSection("cluster").Bind(options);
                        })
                        .Configure<EndpointOptions>(options =>
                        {
                            var endpointConfig = config.GetSection("endpoint");
                            endpointConfig.Bind(options);

                            var advertisedIPAddress = endpointConfig["advertisedIPAddress"];
                            options.AdvertisedIPAddress = string.IsNullOrEmpty(advertisedIPAddress)
                                ? IPAddressUtil.GetLocalIPAddress()
                                : IPAddress.Parse(advertisedIPAddress);
                        })
                        .UseConsulClustering((ConsulClusteringSiloOptions options) =>
                            config.GetSection("consul").Bind(options))
                        .Configure<GrainCollectionOptions>(options => config.GetSection("grains").Bind(options))
                        .ConfigureApplicationParts(parts =>
                            parts.AddApplicationPart(typeof(CatalogItemGrain).Assembly).WithReferences())
                        .ConfigureLogging(logging => logging.AddSerilog());

                    return builder.Build();
                })
                .AddHostedService<SiloHostedService>();

            services.AddHealthChecks(checks => checks.AddCheck<HealthCheck>("HealthCheck", TimeSpan.FromMinutes(1)));
        }

        public void Configure(IApplicationBuilder app)
        {
            Log.Information("Configuring pipeline...");
            app
                .Run(async context =>
                {
                    await context.Response.WriteAsync("Orleans DEMO session!!!");
                });

        }
    }
}