using System;
using System.IO;
using Demo.Orleans.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Orleans.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfigurationRoot config = builder.Build();

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

            Console.ReadLine();
        }
    }
}