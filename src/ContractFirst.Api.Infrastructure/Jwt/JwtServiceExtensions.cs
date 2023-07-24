using Microsoft.Extensions.DependencyInjection;

namespace ContractFirst.Api.Infrastructure.Jwt;

public static class JwtServiceExtensions
{
    public static IServiceCollection AddJwtService(this IServiceCollection services)
    {
        return services.AddSingleton<JwtService>();
    }
}