using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Attempts.Exceptions;

public class InvalidAttemptSubmissionException(Guid lessonId)
    : AppException(
        "Invalid submission",
        $"The submission for lesson '{lessonId}' must include exactly one answer for each of its questions, with no duplicates.",
        HttpStatusCode.BadRequest);
