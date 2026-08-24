using System.Net;
using CodeTrail.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CodeTrail.Api.ErrorHandling;

// Single place that turns every exception - expected (AppException) or not - into a
// ProblemDetails response. Controllers no longer need a try/catch per exception type:
// they call the service and let business-rule violations bubble up here.
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            AppException appException => (appException.StatusCode, appException.Title, appException.Message),
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred. Please try again later.")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception while processing {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            logger.LogWarning("{ExceptionType} while processing {Method} {Path}: {Message}",
                exception.GetType().Name, httpContext.Request.Method, httpContext.Request.Path, exception.Message);
        }

        httpContext.Response.StatusCode = (int)statusCode;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail
        }, cancellationToken);

        return true;
    }
}
