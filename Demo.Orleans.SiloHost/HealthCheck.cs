using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.HealthChecks;

namespace Demo.Orleans.SiloHost
{
    public class HealthCheck: IHealthCheck
    {
        public ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("OK"));
        }
    }
}
