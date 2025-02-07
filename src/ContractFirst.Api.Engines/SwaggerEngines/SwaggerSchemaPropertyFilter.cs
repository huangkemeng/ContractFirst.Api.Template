﻿using System.Reflection;
using System.Text.Json.Serialization;
using ContractFirst.Api.Primary.Bases;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ContractFirst.Api.Engines.SwaggerEngines;

public class SwaggerSchemaPropertyFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties is { Count: > 0 } && context.Type is not null)
        {
            var properties = context.Type.GetProperties();
            foreach (var property in properties)
            {
                if (property.HasAttribute<SwaggerSchemaIgnoreAttribute>())
                {
                    var key = schema.Properties.Keys.FirstOrDefault(x =>
                        (property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name).Equals(x,
                            StringComparison.OrdinalIgnoreCase));

                    if (key is not null)
                    {
                        schema.Properties.Remove(key);
                    }
                }
            }
        }
    }
}