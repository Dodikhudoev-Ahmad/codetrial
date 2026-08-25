using CodeTrail.Application.Profile;
using CodeTrail.Application.Profile.Dtos;
using CodeTrail.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeTrail.Infrastructure.Profile;

public class ProfileService(CodeTrailDbContext db) : IProfileService
{
    public async Task<List<CourseProgressDto>> GetProgressAsync(Guid userId)
    {
        var enrollments = await db.Enrollments
            .Where(e => e.UserId == userId)
            .Include(e => e.Course).ThenInclude(c => c.Lessons)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();

        var passedLessonIds = (await db.LessonAttempts
            .Where(a => a.UserId == userId && a.IsPassed)
            .Select(a => a.LessonId)
            .Distinct()
            .ToListAsync())
            .ToHashSet();

        return enrollments.Select(e => new CourseProgressDto
        {
            CourseId = e.CourseId,
            CourseTitle = e.Course.Title,
            CourseSlug = e.Course.Slug,
            TotalLessons = e.Course.Lessons.Count,
            PassedLessons = e.Course.Lessons.Count(l => passedLessonIds.Contains(l.Id)),
            EnrolledAt = e.EnrolledAt,
            CompletedAt = e.CompletedAt
        }).ToList();
    }
}
