using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Auth.Exceptions;

public class InvalidCredentialsException()
    : AppException("Invalid credentials", "Email or password is incorrect.", HttpStatusCode.Unauthorized);
