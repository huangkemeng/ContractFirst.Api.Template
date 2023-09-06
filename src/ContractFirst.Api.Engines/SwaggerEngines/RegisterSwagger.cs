﻿using System.Text.RegularExpressions;
using ContractFirst.Api.Engines.Bases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace ContractFirst.Api.Engines.SwaggerEngines;

public class RegisterSwagger : IBuilderEngine
{
    private readonly IServiceCollection services;

    public RegisterSwagger(IServiceCollection services)
    {
        this.services = services;
    }

    public void Run()
    {
        services.AddSwaggerGen(options =>
        {
            typeof(SwaggerApiGroupNames).GetFields().Skip(1).ToList().ForEach(f =>
            {
                var info = f.GetCustomAttributes(typeof(SwaggerGroupInfoAttribute), false)
                    .OfType<SwaggerGroupInfoAttribute>().FirstOrDefault();
                options.SwaggerDoc(f.Name, new OpenApiInfo
                {
                    Title = info?.Title,
                    Version = info?.Version,
                    Description = info?.Description
                });
            });

            options.SwaggerDoc("Other", new OpenApiInfo
            {
                Title = "其他"
            });

            options.DocInclusionPredicate((docName, apiDescription) =>
            {
                if (docName == "Other") return string.IsNullOrEmpty(apiDescription.GroupName);

                if (docName == apiDescription.GroupName)
                    return true;
                if (apiDescription.GroupName == "*")
                    if (Enum.TryParse(docName, out SwaggerApiGroupNames groupName))
                    {
                        var fieldInfo = typeof(SwaggerApiGroupNames).GetField(docName)!;
                        var info = fieldInfo.GetCustomAttributes(typeof(SwaggerGroupInfoAttribute), false)
                            .OfType<SwaggerGroupInfoAttribute>().FirstOrDefault();
                        if (info != null && info.MatchRule != null && apiDescription.RelativePath != null)
                        {
                            var matched = new Regex(info.MatchRule).Match(apiDescription.RelativePath);
                            return matched.Success;
                        }
                    }

                return false;
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
            var basePath = AppContext.BaseDirectory;
            options.IncludeXmlComments(Path.Combine(basePath, "ContractFirst.Api.xml"), true);
            options.IncludeXmlComments(Path.Combine(basePath, "ContractFirst.Api.Primary.xml"), true);
            options.IncludeXmlComments(Path.Combine(basePath, "ContractFirst.Api.Infrastructure.xml"), true);
            options.SchemaFilter<DisplayEnumDescFilter>();
        });
    }
}