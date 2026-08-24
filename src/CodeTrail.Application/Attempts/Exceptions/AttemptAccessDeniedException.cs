namespace CodeTrail.Application.Attempts.Exceptions;

public class AttemptAccessDeniedException(Guid attemptId)
    : Exception($"Attempt '{attemptId}' does not belong to the current user.");
