using System.Reflection;
using LockerService.API.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LockerService.API.Filters;

public class TrimPropertiesActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var argument in context.ActionArguments)
        {
            if (argument.Value == null)
            {
                return;
            }
            
            TrimProperties(argument.Value);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    private void TrimProperties(object? obj)
    {
        if (obj == null)
        {
            return;
        }
        
        var properties = obj.GetType().GetProperties();

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<TrimmedValueAttribute>();

            if (attribute == null)
            {
                continue;
            }

            if (property.GetType().Equals(typeof(string)))
            {
                property.SetValue(obj, property.GetValue(obj));
            }

            if (property.GetType().Equals(typeof(IRequest)))
            {
                var valueObject = property.GetValue(obj);
                TrimProperties(valueObject); // Recursively call the same function for nested objects.
            }
        }
    }
}