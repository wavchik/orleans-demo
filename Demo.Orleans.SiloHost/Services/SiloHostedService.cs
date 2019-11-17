using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Orleans.Hosting;

namespace Demo.Orleans.SiloHost.Services
{
    public class SiloHostedService : IHostedService
    {
        private readonly ISiloHost _silo;

        public SiloHostedService(ISiloHost silo)
        {
            _silo = silo;
        }

        public Task StartAsync(CancellationToken cancellationToken) =>
            _silo.StartAsync();

        public Task StopAsync(CancellationToken cancellationToken) =>
            _silo.StopAsync();
    }
}
