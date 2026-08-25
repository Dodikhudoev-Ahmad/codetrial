using CodeTrail.Application.Admin;
using CodeTrail.Application.Admin.Dtos;
using CodeTrail.Application.Admin.Exceptions;
using CodeTrail.Application.Common;
using CodeTrail.Application.Courses.Exceptions;
using CodeTrail.Domain.Entities;
using CodeTrail.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CodeTrail.Infrastructure.Admin;

public class AdminCourseService(CodeTrailDbContext db, ILogger<AdminCourseService> logger) : IAdminCourseService
{
    public async Task<PagedResult<AdminCourseListItemDto>> GetCoursesAsync(int page, int pageSize)
    {
        var query = db.Courses.Include(c => c.Lessons).OrderBy(c => c.Title);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new AdminCourseListItemDto
            {
                Id = c.Id,
                Title = c.Title,
                Slug = c.Slug,
                Level = c.Level,
                Language = c.Language,
                IsPublished = c.IsPublished,
                LessonsCount = c.Lessons.Count
            })
            .ToListAsync();

        return new PagedResult<AdminCourseListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<AdminCourseDetailDto> GetCourseAsync(Guid id)
    {
        var course = await LoadCourseWithContentAsync(id);
        return MapToDetail(course);
    }

    public async Task<AdminCourseDetailDto> CreateCourseAsync(Guid authorId, UpsertCourseRequest request)
    {
        var course = new Course
        {
            Title = request.Title.Trim(),
            Slug = request.Slug.Trim(),
            Description = request.Description.Trim(),
            Level = request.Level,
            Language = request.Language.Trim(),
            // A brand-new course has no lessons yet, so it can never satisfy the
            // publish requirement (rule 9) - IsPublished is ignored on create.
            IsPublished = false,
            AuthorId = authorId
        };

        db.Courses.Add(course);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new CourseTitleOrSlugInUseException();
        }

        logger.LogInformation("Admin {AuthorId} created course {CourseId}", authorId, course.Id);

        return MapToDetail(course);
    }

    public async Task<AdminCourseDetailDto> UpdateCourseAsync(Guid id, UpsertCourseRequest request)
    {
        var course = await LoadCourseWithContentAsync(id);

        if (request.IsPublished && !CanPublish(course))
        {
            throw new CourseNotPublishableException(id);
        }

        course.Title = request.Title.Trim();
        course.Slug = request.Slug.Trim();
        course.Description = request.Description.Trim();
        course.Level = request.Level;
        course.Language = request.Language.Trim();
        course.IsPublished = request.IsPublished;

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new CourseTitleOrSlugInUseException();
        }

        return MapToDetail(course);
    }

    public async Task DeleteCourseAsync(Guid id)
    {
        var course = await db.Courses.FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new CourseNotFoundException(id.ToString());

        db.Courses.Remove(course);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new CannotDeleteWithAttemptsException("Course", id);
        }

        logger.LogInformation("Course {CourseId} deleted", id);
    }

    public async Task<CourseStatsDto> GetCourseStatsAsync(Guid id)
    {
        var course = await LoadCourseWithContentAsync(id);

        var enrollmentsCount = await db.Enrollments.CountAsync(e => e.CourseId == id);
        var completionsCount = await db.Enrollments.CountAsync(e => e.CourseId == id && e.CompletedAt != null);

        var lessonIds = course.Lessons.Select(l => l.Id).ToList();
        var attempts = await db.LessonAttempts
            .Where(a => lessonIds.Contains(a.LessonId))
            .Select(a => new { a.LessonId, a.UserId, a.ScorePercent, a.IsPassed })
            .ToListAsync();

        var lessonStats = course.Lessons
            .OrderBy(l => l.Order)
            .Select(l =>
            {
                var lessonAttempts = attempts.Where(a => a.LessonId == l.Id).ToList();

                return new LessonStatsDto
                {
                    LessonId = l.Id,
                    LessonTitle = l.Title,
                    AttemptsCount = lessonAttempts.Count,
                    StudentsPassedCount = lessonAttempts.Where(a => a.IsPassed).Select(a => a.UserId).Distinct().Count(),
                    AverageScorePercent = lessonAttempts.Count > 0
                        ? Math.Round(lessonAttempts.Average(a => a.ScorePercent), 1)
                        : 0
                };
            })
            .ToList();

        return new CourseStatsDto
        {
            CourseId = course.Id,
            CourseTitle = course.Title,
            EnrollmentsCount = enrollmentsCount,
            CompletionsCount = completionsCount,
            AverageScorePercent = attempts.Count > 0 ? Math.Round(attempts.Average(a => a.ScorePercent), 1) : 0,
            Lessons = lessonStats
        };
    }

    private async Task<Course> LoadCourseWithContentAsync(Guid id) =>
        await db.Courses
            .Include(c => c.Lessons).ThenInclude(l => l.Questions)
            .FirstOrDefaultAsync(c => c.Id == id)
        ?? throw new CourseNotFoundException(id.ToString());

    private static bool CanPublish(Course course) =>
        course.Lessons.Count > 0 && course.Lessons.All(l => l.Questions.Count > 0);

    private static AdminCourseDetailDto MapToDetail(Course course) => new()
    {
        Id = course.Id,
        Title = course.Title,
        Slug = course.Slug,
        Description = course.Description,
        Level = course.Level,
        Language = course.Language,
        IsPublished = course.IsPublished,
        Lessons = course.Lessons
            .OrderBy(l => l.Order)
            .Select(l => new AdminLessonSummaryDto
            {
                Id = l.Id,
                Order = l.Order,
                Title = l.Title,
                XpReward = l.XpReward,
                QuestionsCount = l.Questions.Count
            })
            .ToList()
    };
}
