using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LockerService.API.Filters;

public class AuthorizationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes = context.MethodInfo?.DeclaringType?.GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true))
            .OfType<AuthorizeAttribute>()
            .ToList();

        if (attributes == null || !attributes.Any())
        {
            // operation.Security.Clear();
            return;
        }

        var attr = attributes[0];
        var securityInfos = new List<string>();
        securityInfos.Add($"{nameof(AuthorizeAttribute.Policy)}:{attr.Policy}");
        securityInfos.Add($"{nameof(AuthorizeAttribute.Roles)}:{attr.Roles}");
        securityInfos.Add($"{nameof(AuthorizeAttribute.AuthenticationSchemes)}:{attr.AuthenticationSchemes}");

        switch (attr.AuthenticationSchemes)
        {
            case JwtBearerDefaults.AuthenticationScheme:
            default:
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
                                    Id = "Bearer"
                                },
                                In = ParameterLocation.Header
                            },
                            securityInfos
                        },
                    }
                };
                break;
        }
    }
}