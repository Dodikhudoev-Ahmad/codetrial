namespace CodeTrail.Application.Auth.Exceptions;

public class UserNotFoundException(Guid userId) : Exception($"User '{userId}' was not found.");
