using LockerService.Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LockerService.API.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute : Attribute, IAsyncActionFilter
{
    private const string ApiKey = "X-API-KEY";
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKey, out var apiKeyVal))
        {
            context.HttpContext.Response.StatusCode = 401;
            await context.HttpContext.Response.WriteAsync("API Key not found");
            return;
        }
        
        var apiSettings = context.HttpContext.RequestServices.GetRequiredService<ApiKeySettings>();
        if (!apiKeyVal.Equals(apiSettings.Key))
        {
            context.HttpContext.Response.StatusCode = 401;
            await context.HttpContext.Response.WriteAsync("Invalid API key");
            return;
        }

        await next();
    }
}