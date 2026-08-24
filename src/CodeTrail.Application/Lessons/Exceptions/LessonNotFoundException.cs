namespace CodeTrail.Application.Lessons.Exceptions;

public class LessonNotFoundException(Guid lessonId) : Exception($"Lesson '{lessonId}' was not found.");
