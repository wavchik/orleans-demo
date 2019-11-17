using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;

namespace Demo.Orleans.SiloHost
{
    public class Program
    {
        public static void Main(string[] args) =>
            BuildWebHost(args).Build().Run();

        public static IWebHostBuilder BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((webHostBuilderContext, configurationBuilder) =>
                {
                    configurationBuilder
                        .AddJsonFile("appsettings.json", optional: true);
                    configurationBuilder.AddEnvironmentVariables();
                })
                .UseStartup<Startup>();
    }
}
