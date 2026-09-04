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

        var watchedPercent = await db.VideoProgress
            .Where(v => v.LessonId == lessonId && v.UserId == userId)
            .Select(v => v.WatchedPercent)
            .FirstOrDefaultAsync();

        return MapToDetailDto(lesson, watchedPercent);
    }

    public async Task<VideoProgressDto> UpdateVideoProgressAsync(Guid lessonId, Guid userId, int watchedPercent)
    {
        var lesson = await db.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId)
            ?? throw new LessonNotFoundException(lessonId);

        await accessGuard.EnsureUnlockedAsync(lessonId, lesson.CourseId, userId);

        var clamped = Math.Clamp(watchedPercent, 0, 100);
        var progress = await db.VideoProgress.FirstOrDefaultAsync(v => v.LessonId == lessonId && v.UserId == userId);

        if (progress is null)
        {
            progress = new VideoProgress { UserId = userId, LessonId = lessonId, WatchedPercent = clamped, UpdatedAt = DateTime.UtcNow };
            db.VideoProgress.Add(progress);
            await db.SaveChangesAsync();
        }
        else if (clamped > progress.WatchedPercent)
        {
            // Progress only ever moves forward - a viewer seeking backward to rewatch a
            // section shouldn't undo credit for what they already watched.
            progress.WatchedPercent = clamped;
            progress.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        return new VideoProgressDto { WatchedPercent = progress.WatchedPercent };
    }

    private static LessonDetailDto MapToDetailDto(Lesson lesson, int videoWatchedPercent) => new()
    {
        Id = lesson.Id,
        CourseId = lesson.CourseId,
        Order = lesson.Order,
        Title = lesson.Title,
        TheoryMarkdown = lesson.TheoryMarkdown,
        XpReward = lesson.XpReward,
        YouTubeVideoId = lesson.YouTubeVideoId,
        VideoWatchedPercent = videoWatchedPercent,
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
