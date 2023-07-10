using LockerService.API.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LockerService.API.Filters;

public class ApiKeyOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes = context.MethodInfo?.DeclaringType?.GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true))
            .OfType<ApiKeyAttribute>()
            .ToList();

        if (attributes == null || !attributes.Any())
            // operation.Security.Clear();
            return;

        var attr = attributes[0];
        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "X-API-KEY"
                        },
                        In = ParameterLocation.Header
                    },
                    new string[] { }
                }
            }
        };
    }
}

