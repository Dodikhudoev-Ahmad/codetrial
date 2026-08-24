using CodeTrail.Application.Courses;
using CodeTrail.Application.Courses.Dtos;
using CodeTrail.Application.Lessons;
using CodeTrail.Application.Lessons.Exceptions;
using CodeTrail.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeTrail.Infrastructure.Lessons;

public class LessonAccessGuard(CodeTrailDbContext db) : ILessonAccessGuard
{
    public async Task EnsureUnlockedAsync(Guid lessonId, Guid courseId, Guid userId)
    {
        var isEnrolled = await db.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (!isEnrolled)
        {
            throw new NotEnrolledException(courseId);
        }

        var courseLessons = await db.Lessons
            .Where(l => l.CourseId == courseId)
            .Select(l => new { l.Id, l.Order })
            .ToListAsync();

        var passedLessonIds = (await db.LessonAttempts
            .Where(a => a.UserId == userId && a.Lesson.CourseId == courseId && a.IsPassed)
            .Select(a => a.LessonId)
            .Distinct()
            .ToListAsync())
            .ToHashSet();

        var statuses = LessonUnlockCalculator.ComputeStatuses(
            courseLessons.Select(l => (l.Id, l.Order)), isEnrolled, passedLessonIds);

        if (statuses[lessonId] == LessonStatus.Locked)
        {
            throw new LessonLockedException(lessonId);
        }
    }
}
