using System.Net;
using System.Text.Json;
using FluentValidation;
using LockerService.Application.Common.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LockerService.Application.Common.Exceptions;

public class ExceptionMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException exception)
        {
            _logger.LogError(exception, exception.Message);
            await HandleApiExceptionAsync(context, exception);
        }
        catch (ValidationException exception)
        {
            _logger.LogError(exception, exception.Message);
            await HandleValidationExceptionASync(context, exception);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            await HandleInternalServerExceptionAsync(context, exception);
        }
    }

    private async Task HandleInternalServerExceptionAsync(HttpContext context, Exception exception)
    {
        await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, new ExceptionResponse(exception));
    }

    private async Task HandleValidationExceptionASync(HttpContext context, ValidationException exception)
    {
        await HandleExceptionAsync(context, HttpStatusCode.BadRequest, new ExceptionResponse(exception));
    }

    private async Task HandleApiExceptionAsync(HttpContext context, ApiException exception)
    {
        await HandleExceptionAsync(context, HttpStatusCode.BadRequest, new ExceptionResponse(exception));
    }

    private async Task HandleExceptionAsync(HttpContext context, HttpStatusCode code, ExceptionResponse response)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}