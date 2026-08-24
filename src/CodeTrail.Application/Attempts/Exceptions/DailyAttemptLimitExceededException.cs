using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Attempts.Exceptions;

public class DailyAttemptLimitExceededException(Guid lessonId)
    : AppException(
        "Daily attempt limit reached",
        $"The maximum of 5 attempts for lesson '{lessonId}' today has been reached. Try again tomorrow.",
        HttpStatusCode.Forbidden);
