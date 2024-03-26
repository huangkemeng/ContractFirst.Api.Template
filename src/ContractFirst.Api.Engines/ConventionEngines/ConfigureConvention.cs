using System.Globalization;
using System.Security.Claims;
using System.Text;
using ContractFirst.Api.Engines.Bases;
using ContractFirst.Api.Infrastructure.CorsFunction;
using ContractFirst.Api.Infrastructure.JwtFunction;
using ContractFirst.Api.Realization.Bases;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace ContractFirst.Api.Engines.ConventionEngines;

/// <summary>
///     一些常规的配置
/// </summary>
public class ConfigureConvention : IBuilderEngine
{
    private readonly IServiceCollection services;

    public ConfigureConvention(IServiceCollection services)
    {
        this.services = services;
    }

    public void Run()
    {
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddControllers();
        services.AddRouting(e => { e.LowercaseUrls = true; });
        services.AddEndpointsApiExplorer();
        services.AddCorsFunction();
        services.AddJwtFunction();
        services.AddExceptionHandler<GlobalExceptionHandler>();
    }
}

public class UseConvention : IAppEngine
{
    private readonly WebApplication app;

    public UseConvention(WebApplication app)
    {
        this.app = app;
    }

    public void Run()
    {
        app.UseHttpsRedirection();
        app.MapControllers();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        ValidatorOptions.Global.LanguageManager.Enabled = true;
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("zh-CN");
    }
}