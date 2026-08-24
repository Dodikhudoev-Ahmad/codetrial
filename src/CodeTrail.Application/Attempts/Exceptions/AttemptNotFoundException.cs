namespace CodeTrail.Application.Attempts.Exceptions;

public class AttemptNotFoundException(Guid attemptId) : Exception($"Attempt '{attemptId}' was not found.");
