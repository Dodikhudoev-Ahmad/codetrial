namespace CodeTrail.Application.Attempts.Exceptions;

public class DailyAttemptLimitExceededException(Guid lessonId)
    : Exception($"The maximum of 5 attempts for lesson '{lessonId}' today has been reached. Try again tomorrow.");
