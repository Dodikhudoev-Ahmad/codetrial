using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Admin.Exceptions;

public class QuestionNotFoundException(Guid questionId)
    : AppException("Question not found", $"Question '{questionId}' was not found.", HttpStatusCode.NotFound);
