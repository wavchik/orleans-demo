using Demo.SiloHost.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Patterns.SmartCache.Host
{
    public class Program
    {
        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddOrleansClient(config)
                .AddSingleton<SmartCacheDemo>()
                .AddSingleton<RegistryDemo>()
                .BuildServiceProvider();

            serviceProvider
                .GetRequiredService<SmartCacheDemo>()
                .Run();

            serviceProvider
                .GetRequiredService<RegistryDemo>()
                .Run();
        }
    }
}