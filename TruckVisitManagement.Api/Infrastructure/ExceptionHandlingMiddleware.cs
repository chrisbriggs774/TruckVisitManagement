using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TruckVisitManagement.Application.Commands;

namespace TruckVisitManagement.Api.Infrastructure;

/// <summary>
/// Translates domain and application exceptions into RFC 7807 ProblemDetails responses.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var (status, title) = exception switch
        {
            VisitNotFoundException => (StatusCodes.Status404NotFound, "Visit not found"),
            InvalidVisitStatusTransitionException => (StatusCodes.Status409Conflict, "Invalid status transition"),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Operation not allowed"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception processing request.");
        }
        else
        {
            _logger.LogWarning(exception, "Request failed with status {Status}.", status);
        }

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = status;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
