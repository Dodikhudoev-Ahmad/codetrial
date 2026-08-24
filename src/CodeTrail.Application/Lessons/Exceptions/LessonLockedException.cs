namespace CodeTrail.Application.Lessons.Exceptions;

public class LessonLockedException(Guid lessonId)
    : Exception($"Lesson '{lessonId}' is locked. Complete the previous lesson first.");
