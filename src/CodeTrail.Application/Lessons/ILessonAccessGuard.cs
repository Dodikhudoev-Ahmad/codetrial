namespace CodeTrail.Application.Lessons;

// Enforces business rules 1 and 3 (sequential unlocking, enrollment required) wherever
// a lesson is accessed - both for reading its content and for submitting an attempt.
public interface ILessonAccessGuard
{
    Task EnsureUnlockedAsync(Guid lessonId, Guid courseId, Guid userId);
}
