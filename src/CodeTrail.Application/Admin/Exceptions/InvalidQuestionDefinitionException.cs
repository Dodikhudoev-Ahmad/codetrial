using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Admin.Exceptions;

public class InvalidQuestionDefinitionException(string reason)
    : AppException("Invalid question definition", reason, HttpStatusCode.BadRequest);
