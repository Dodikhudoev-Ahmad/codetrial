using CodeTrail.Application.Lessons;
using CodeTrail.Application.Lessons.Dtos;
using CodeTrail.Application.Lessons.Exceptions;
using CodeTrail.Domain.Entities;
using CodeTrail.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeTrail.Infrastructure.Lessons;

public class LessonService(CodeTrailDbContext db, ILessonAccessGuard accessGuard) : ILessonService
{
    public async Task<LessonDetailDto> GetLessonAsync(Guid lessonId, Guid userId)
    {
        var lesson = await db.Lessons
            .Include(l => l.Questions.OrderBy(q => q.Order))
                .ThenInclude(q => q.AnswerOptions)
            .FirstOrDefaultAsync(l => l.Id == lessonId)
            ?? throw new LessonNotFoundException(lessonId);

        await accessGuard.EnsureUnlockedAsync(lessonId, lesson.CourseId, userId);

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
        YouTubeVideoId = lesson.YouTubeVideoId,
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
