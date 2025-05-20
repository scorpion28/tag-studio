using System.Net;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TagStudio.Tags.Domain.Exceptions;

namespace TagStudio.WebApi.Common;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var title = "An unexpected error occurred.";

        switch (exception)
        {
            case ForbiddenAccessException:
                statusCode = HttpStatusCode.Forbidden;
                title = "Forbidden";
                break;

            case NotFoundException e:
                logger.LogError("{ErrorMessage}", e.Message);
                statusCode = HttpStatusCode.NotFound;
                title = "Not Found";
                break;
            default:
                logger.LogError(exception, "Unhandled error occurred");
                break;
        }

        httpContext.Response.StatusCode = (int)statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = (int)statusCode,
            Title = title,
        }, cancellationToken);

        return true;
    }
}