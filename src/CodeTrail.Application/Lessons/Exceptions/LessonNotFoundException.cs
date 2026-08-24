using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Lessons.Exceptions;

public class LessonNotFoundException(Guid lessonId)
    : AppException("Lesson not found", $"Lesson '{lessonId}' was not found.", HttpStatusCode.NotFound);
