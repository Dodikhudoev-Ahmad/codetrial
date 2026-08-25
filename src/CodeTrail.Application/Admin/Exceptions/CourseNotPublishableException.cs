using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Admin.Exceptions;

public class CourseNotPublishableException(Guid courseId)
    : AppException(
        "Course cannot be published",
        $"Course '{courseId}' cannot be published: it must have at least one lesson, and every lesson must have at least one question.",
        HttpStatusCode.BadRequest);
