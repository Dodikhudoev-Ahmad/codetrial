using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Auth.Exceptions;

public class EmailAlreadyInUseException(string email)
    : AppException("Email already in use", $"Email '{email}' is already registered.", HttpStatusCode.Conflict);
