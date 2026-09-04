using CodeTrail.Application.Common;
using CodeTrail.Application.Courses;
using CodeTrail.Application.Courses.Dtos;
using CodeTrail.Application.Courses.Exceptions;
using CodeTrail.Domain.Entities;
using CodeTrail.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CodeTrail.Infrastructure.Courses;

public class CourseService(CodeTrailDbContext db, ILogger<CourseService> logger) : ICourseService
{
    public async Task<PagedResult<CourseSummaryDto>> GetCoursesAsync(CourseListQuery query)
    {
        var courses = db.Courses.Where(c => c.IsPublished);

        if (query.Level.HasValue)
        {
            courses = courses.Where(c => c.Level == query.Level.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Language))
        {
            courses = courses.Where(c => c.Language == query.Language);
        }

        // Free-text search is done in memory rather than translated to SQL: SQLite's
        // built-in LIKE/LOWER only case-fold ASCII, which would silently break search
        // for the Cyrillic course titles/descriptions this catalog actually has. The
        // catalog is small by design (a handful of courses), so this trade-off is cheap.
        var candidates = await courses.Include(c => c.Lessons).ToListAsync();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            candidates = candidates
                .Where(c => c.Title.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || c.Description.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var totalCount = candidates.Count;

        var items = candidates
            .OrderBy(c => c.Title, StringComparer.Ordinal)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new CourseSummaryDto
            {
                Id = c.Id,
                Title = c.Title,
                Slug = c.Slug,
                Description = c.Description,
                Level = c.Level,
                Language = c.Language,
                LessonsCount = c.Lessons.Count
            })
            .ToList();

        return new PagedResult<CourseSummaryDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<CourseDetailDto> GetCourseBySlugAsync(string slug, Guid? currentUserId)
    {
        var course = await db.Courses
            .Include(c => c.Lessons.OrderBy(l => l.Order))
            .FirstOrDefaultAsync(c => c.Slug == slug && c.IsPublished)
            ?? throw new CourseNotFoundException(slug);

        // A student must be enrolled before any lesson counts as unlocked (business rule 3),
        // so an anonymous or non-enrolled visitor sees every lesson as locked.
        var isEnrolled = currentUserId.HasValue
            && await db.Enrollments.AnyAsync(e => e.UserId == currentUserId.Value && e.CourseId == course.Id);

        var passedLessonIds = isEnrolled
            ? (await db.LessonAttempts
                .Where(a => a.UserId == currentUserId!.Value && a.Lesson.CourseId == course.Id && a.IsPassed)
                .Select(a => a.LessonId)
                .Distinct()
                .ToListAsync())
                .ToHashSet()
            : [];

        return new CourseDetailDto
        {
            Id = course.Id,
            Title = course.Title,
            Slug = course.Slug,
            Description = course.Description,
            Level = course.Level,
            Language = course.Language,
            IsEnrolled = isEnrolled,
            Lessons = BuildLessonSummaries(course.Lessons, isEnrolled, passedLessonIds)
        };
    }

    public async Task<EnrollmentDto> EnrollAsync(Guid courseId, Guid userId)
    {
        var courseExists = await db.Courses.AnyAsync(c => c.Id == courseId && c.IsPublished);

        if (!courseExists)
        {
            throw new CourseNotFoundException(courseId.ToString());
        }

        if (await db.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId))
        {
            throw new AlreadyEnrolledException(courseId);
        }

        var enrollment = new Enrollment
        {
            UserId = userId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow
        };

        db.Enrollments.Add(enrollment);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Race between two concurrent enroll requests for the same user/course.
            throw new AlreadyEnrolledException(courseId);
        }

        logger.LogInformation("User {UserId} enrolled in course {CourseId}", userId, courseId);

        return new EnrollmentDto { CourseId = courseId, EnrolledAt = enrollment.EnrolledAt };
    }

    private static List<LessonSummaryDto> BuildLessonSummaries(
        IEnumerable<Lesson> lessons, bool isEnrolled, ICollection<Guid> passedLessonIds)
    {
        var lessonList = lessons.OrderBy(l => l.Order).ToList();
        var statuses = LessonUnlockCalculator.ComputeStatuses(
            lessonList.Select(l => (l.Id, l.Order)), isEnrolled, passedLessonIds);

        return lessonList.Select(l => new LessonSummaryDto
        {
            Id = l.Id,
            Order = l.Order,
            Title = l.Title,
            XpReward = l.XpReward,
            HasVideo = !string.IsNullOrEmpty(l.YouTubeVideoId),
            Status = statuses[l.Id]
        }).ToList();
    }
}
