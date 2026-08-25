using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Admin.Exceptions;

public class QuestionOrderConflictException(Guid lessonId, int order)
    : AppException(
        "Question order conflict",
        $"Lesson '{lessonId}' already has a question with order {order}.",
        HttpStatusCode.Conflict);
