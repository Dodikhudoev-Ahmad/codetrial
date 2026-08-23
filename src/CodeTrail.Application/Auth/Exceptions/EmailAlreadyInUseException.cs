namespace CodeTrail.Application.Auth.Exceptions;

public class EmailAlreadyInUseException(string email) : Exception($"Email '{email}' is already registered.");
