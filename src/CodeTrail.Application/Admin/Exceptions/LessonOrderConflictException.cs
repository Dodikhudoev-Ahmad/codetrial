using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Admin.Exceptions;

public class LessonOrderConflictException(Guid courseId, int order)
    : AppException(
        "Lesson order conflict",
        $"Course '{courseId}' already has a lesson with order {order}.",
        HttpStatusCode.Conflict);
