using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ContractFirst.Api.Infrastructure.Bases;

public class Status200HealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}