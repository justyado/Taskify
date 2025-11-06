using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Taskify.Api.Middlewares;

internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var requestId = httpContext.Items["RequestId"]?.ToString() ?? Guid.NewGuid().ToString();
        var requestPath = httpContext.Request.Path;
        var requestMethod = httpContext.Request.Method;

        // Определяем уровень логирования в зависимости от типа исключения
        var logLevel = exception switch
        {
            ValidationException => LogLevel.Warning,
            ApplicationException => LogLevel.Warning,
            _ => LogLevel.Error
        };

        // Логируем с дополнительным контекстом
        logger.Log(
            logLevel,
            exception,
            "Unhandled exception occurred. RequestId: {RequestId}, Method: {Method}, Path: {Path}, ExceptionType: {ExceptionType}",
            requestId,
            requestMethod,
            requestPath,
            exception.GetType().Name);

        var problemDetails = new ProblemDetails
        {
            Type = exception.GetType().Name,
            Title = "An error occurred",
            Status = exception switch
            {
                ApplicationException => StatusCodes.Status400BadRequest,
                ValidationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            }
        };

        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            problemDetails.Detail = "One or more validation errors occurred.";
            problemDetails.Extensions["errors"] = errors;

            logger.LogWarning(
                "Validation failed. RequestId: {RequestId}, Errors: {@ValidationErrors}",
                requestId,
                errors);
        }
        else
        {
            problemDetails.Detail = exception.Message;

            // Дополнительное логирование для критических ошибок
            if (exception is not ApplicationException)
            {
                logger.LogError(
                    "Critical error details - RequestId: {RequestId}, StackTrace: {StackTrace}, InnerException: {InnerException}",
                    requestId,
                    exception.StackTrace,
                    exception.InnerException?.Message);
            }
        }

        problemDetails.Extensions["requestId"] = requestId;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        var result = await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });

        if (result)
        {
            logger.LogDebug("Problem details written successfully for RequestId: {RequestId}", requestId);
        }

        return result;
    }
}
