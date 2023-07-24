using System.Globalization;
using System.Security.Claims;
using System.Text;
using ContractFirst.Api.Engines.Bases;
using ContractFirst.Api.Infrastructure.Jwt;
using ContractFirst.Api.Infrastructure.Settings;
using ContractFirst.Api.Realization.Bases;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace ContractFirst.Api.Engines.ConventionEngines;

/// <summary>
/// 一些常规的配置
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
        AddCors();
        AddJwt();
    }

    private void AddCors()
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var corSetting = serviceProvider.GetService<CorsSetting>();
                var environment = serviceProvider.GetService<IWebHostEnvironment>();
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowedToAllowWildcardSubdomains();
                if (environment?.IsProduction() == true)
                    policy.WithOrigins(corSetting?.Origins ?? new string[] { })
                        .AllowCredentials();
                else
                    policy.AllowAnyOrigin();
            });
        });
    }

    private void AddJwt()
    {
        services.AddJwtService()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var provider = services.BuildServiceProvider();
                var jwtSetting = provider.GetService<JwtSetting>() ??
                                 throw new BusinessException($"获取配置文件{nameof(JwtSetting)}失败",
                                     BusinessExceptionTypeEnum.Configuration);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSetting.Issuer,
                    ValidAudience = jwtSetting.Audience,
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.SignKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var principal = context.Principal;
                        if (principal != null)
                        {
                            var tokenType = principal.FindFirstValue(JwtService.TokenTypeConst);
                            if (tokenType == TokenTypeEnum.AccessToken.ToString()) return Task.CompletedTask;
                        }

                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    },
                    OnForbidden = context => { return Task.CompletedTask; },
                    OnAuthenticationFailed = context => { return Task.CompletedTask; },
                    OnChallenge = context => { return Task.CompletedTask; },
                    OnMessageReceived = context => { return Task.CompletedTask; }
                };
            });
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
        app.Use((context, next) =>
        {
            context.Response.Headers.Add("Access-Control-Expose-Headers", "www-authenticate");
            return next();
        });
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        ValidatorOptions.Global.LanguageManager.Enabled = true;
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("zh-CN");
    }
}