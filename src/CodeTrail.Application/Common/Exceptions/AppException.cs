using System.Net;

namespace CodeTrail.Application.Common.Exceptions;

// Base for every business-rule exception the application throws deliberately (as
// opposed to a genuine bug). Carries its own HTTP status and a short, stable title so
// the global exception handler can translate it to a ProblemDetails response without
// controllers needing a try/catch per exception type.
public abstract class AppException(string title, string message, HttpStatusCode statusCode) : Exception(message)
{
    public string Title { get; } = title;
    public HttpStatusCode StatusCode { get; } = statusCode;
}
