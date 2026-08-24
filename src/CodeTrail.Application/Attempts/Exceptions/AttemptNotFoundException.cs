using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Attempts.Exceptions;

public class AttemptNotFoundException(Guid attemptId)
    : AppException("Attempt not found", $"Attempt '{attemptId}' was not found.", HttpStatusCode.NotFound);
