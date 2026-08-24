using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Attempts.Exceptions;

public class AttemptAccessDeniedException(Guid attemptId)
    : AppException(
        "Access denied",
        $"Attempt '{attemptId}' does not belong to the current user.",
        HttpStatusCode.Forbidden);
