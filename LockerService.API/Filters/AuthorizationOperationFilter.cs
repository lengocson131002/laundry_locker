using LockerService.API.Attributes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LockerService.API.Filters;

/// <summary>
/// </summary>
public class AuthorizationOperationFilter : IOperationFilter
{
    /// <summary>
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="context"></param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var securitySchemes = new List<OpenApiSecurityRequirement>();
        
        securitySchemes.AddRange(GetXApiKeySchemes(operation, context));
        securitySchemes.AddRange(GetJwtKeySchemes(operation, context));
        
        operation.Security = securitySchemes;
    }

    private List<OpenApiSecurityRequirement> GetXApiKeySchemes(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes = context.MethodInfo?.DeclaringType?.GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true))
            .OfType<ApiKeyAttribute>()
            .ToList();

        if (attributes == null || !attributes.Any())
        {
            return new List<OpenApiSecurityRequirement>();
        }

       return new List<OpenApiSecurityRequirement>
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
    
    private List<OpenApiSecurityRequirement> GetJwtKeySchemes(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes = context.MethodInfo?.DeclaringType?.GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true))
            .OfType<AuthorizeAttribute>()
            .ToList();

        if (attributes == null || !attributes.Any())
        {
            // operation.Security.Clear();
            return new List<OpenApiSecurityRequirement>();
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
                return new List<OpenApiSecurityRequirement>
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
        }
    }
}