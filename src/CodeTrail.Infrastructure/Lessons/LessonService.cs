using CodeTrail.Application.Courses;
using CodeTrail.Application.Courses.Dtos;
using CodeTrail.Application.Lessons;
using CodeTrail.Application.Lessons.Dtos;
using CodeTrail.Application.Lessons.Exceptions;
using CodeTrail.Domain.Entities;
using CodeTrail.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeTrail.Infrastructure.Lessons;

public class LessonService(CodeTrailDbContext db) : ILessonService
{
    public async Task<LessonDetailDto> GetLessonAsync(Guid lessonId, Guid userId)
    {
        var lesson = await db.Lessons
            .Include(l => l.Questions.OrderBy(q => q.Order))
                .ThenInclude(q => q.AnswerOptions)
            .FirstOrDefaultAsync(l => l.Id == lessonId)
            ?? throw new LessonNotFoundException(lessonId);

        var isEnrolled = await db.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == lesson.CourseId);

        if (!isEnrolled)
        {
            throw new NotEnrolledException(lesson.CourseId);
        }

        var courseLessons = await db.Lessons
            .Where(l => l.CourseId == lesson.CourseId)
            .Select(l => new { l.Id, l.Order })
            .ToListAsync();

        var passedLessonIds = (await db.LessonAttempts
            .Where(a => a.UserId == userId && a.Lesson.CourseId == lesson.CourseId && a.IsPassed)
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

        return MapToDetailDto(lesson);
    }

    private static LessonDetailDto MapToDetailDto(Lesson lesson) => new()
    {
        Id = lesson.Id,
        CourseId = lesson.CourseId,
        Order = lesson.Order,
        Title = lesson.Title,
        TheoryMarkdown = lesson.TheoryMarkdown,
        XpReward = lesson.XpReward,
        Questions = lesson.Questions
            .OrderBy(q => q.Order)
            .Select(q => new QuestionPreviewDto
            {
                Id = q.Id,
                Order = q.Order,
                Type = q.Type,
                Text = q.Text,
                CodeSnippet = q.CodeSnippet,
                Options = q.AnswerOptions
                    .Select(o => new AnswerOptionPreviewDto { Id = o.Id, Text = o.Text })
                    .ToList()
            })
            .ToList()
    };
}
