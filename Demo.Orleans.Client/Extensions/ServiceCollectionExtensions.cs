using Demo.Orleans.Client.Configuration;
using Demo.SmartCache.GrainInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Polly;
using Serilog;

namespace Demo.Orleans.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(typeof(ServiceCollectionExtensions));


        public static IServiceCollection AddOrleansClient(this IServiceCollection t, IConfigurationRoot config)
        {
            Log.Debug("Configuring Orleans client...");

            var client = new ClientBuilder()
                .Configure<ClusterOptions>(config.GetSection("cluster"))
                .UseConsulClustering((ConsulClusteringClientOptions options) =>
                    config.GetSection("consul").Bind(options))
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ICatalogItemGrain).Assembly))
                .ConfigureLogging(logging => logging.AddSerilog())
                .UseDashboard()
                .Build();

            var connectionConfig = config.GetSection("connection").Get<ConnectionConfig>();

            // Attempt to connect a few times to overcome transient failures and to give the silo enough 
            // time to start up when starting at the same time as the client (useful when deploying or during development).
            Policy
                .Handle<SiloUnavailableException>()
                .WaitAndRetry(
                    retryCount: connectionConfig.ConnectionRetriesCount,
                    sleepDurationProvider: (i, ctx) => connectionConfig.ConnectionRetriesTimeout,
                    onRetry: (ex, ts, i, ctx) => Log.Error(ex, $"Establishing connection with Orleans failed, retry count = {i}, retry timeout = {ts}."))
                .Execute(() =>
                {
                    client.Connect().Wait();
                    Log.Information("Connection with Orleans established");
                });

            t.AddSingleton<IGrainFactory>(client);

            Log.Debug("Orleans client configured.");
            return t;
        }
    }
}
