using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Auth.Exceptions;

public class UserNotFoundException(Guid userId)
    : AppException("User not found", $"User '{userId}' was not found.", HttpStatusCode.NotFound);
