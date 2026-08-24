namespace CodeTrail.Application.Attempts.Exceptions;

public class InvalidAttemptSubmissionException(Guid lessonId)
    : Exception($"The submission for lesson '{lessonId}' must include exactly one answer for each of its questions, with no duplicates.");
