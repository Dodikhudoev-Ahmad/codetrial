using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Lessons.Exceptions;

public class NotEnrolledException(Guid courseId)
    : AppException(
        "Not enrolled",
        $"You must enroll in course '{courseId}' before accessing its lessons.",
        HttpStatusCode.Forbidden);
