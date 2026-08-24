using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Lessons.Exceptions;

public class LessonLockedException(Guid lessonId)
    : AppException(
        "Lesson locked",
        $"Lesson '{lessonId}' is locked. Complete the previous lesson first.",
        HttpStatusCode.Forbidden);
