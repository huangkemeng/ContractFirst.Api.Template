﻿using ContractFirst.Api.Primary.Bases;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ContractFirst.Api.Engines.SwaggerEngines;

public class SwaggerQueryPropertyFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var ignoredParameters = context.ApiDescription.ParameterDescriptions
            .Where(x => x.CustomAttributes().Any(c => c is SwaggerSchemaIgnoreAttribute)).ToList();

        foreach (var ignoredParameter in ignoredParameters)
        {
            var parameter = operation.Parameters.FirstOrDefault(parameter =>
                string.Equals(parameter.Name.ToLower(), ignoredParameter.Name.ToLower()));

            if (parameter != null)
            {
                operation.Parameters.Remove(parameter);
            }
        }
    }
}