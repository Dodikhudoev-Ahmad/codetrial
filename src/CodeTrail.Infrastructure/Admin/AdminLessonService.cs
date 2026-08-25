using CodeTrail.Application.Admin;
using CodeTrail.Application.Admin.Dtos;
using CodeTrail.Application.Admin.Exceptions;
using CodeTrail.Application.Courses.Exceptions;
using CodeTrail.Application.Lessons.Exceptions;
using CodeTrail.Domain.Entities;
using CodeTrail.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeTrail.Infrastructure.Admin;

public class AdminLessonService(CodeTrailDbContext db) : IAdminLessonService
{
    public async Task<AdminLessonDetailDto> GetLessonAsync(Guid id)
    {
        var lesson = await LoadLessonWithQuestionsAsync(id);
        return MapToDetail(lesson);
    }

    public async Task<AdminLessonDetailDto> CreateLessonAsync(UpsertLessonRequest request)
    {
        var courseExists = await db.Courses.AnyAsync(c => c.Id == request.CourseId);

        if (!courseExists)
        {
            throw new CourseNotFoundException(request.CourseId.ToString());
        }

        // New lessons are appended at the end of the course - explicit reordering
        // happens through update, where a conflict with an existing sibling is checked.
        var maxOrder = await db.Lessons
            .Where(l => l.CourseId == request.CourseId)
            .Select(l => (int?)l.Order)
            .MaxAsync() ?? 0;

        var lesson = new Lesson
        {
            CourseId = request.CourseId,
            Order = maxOrder + 1,
            Title = request.Title.Trim(),
            TheoryMarkdown = request.TheoryMarkdown,
            XpReward = request.XpReward
        };

        db.Lessons.Add(lesson);
        await db.SaveChangesAsync();

        return MapToDetail(lesson);
    }

    public async Task<AdminLessonDetailDto> UpdateLessonAsync(Guid id, UpsertLessonRequest request)
    {
        var lesson = await LoadLessonWithQuestionsAsync(id);

        lesson.Title = request.Title.Trim();
        lesson.TheoryMarkdown = request.TheoryMarkdown;
        lesson.XpReward = request.XpReward;

        if (request.Order != lesson.Order)
        {
            var conflict = await db.Lessons.AnyAsync(l =>
                l.CourseId == lesson.CourseId && l.Order == request.Order && l.Id != lesson.Id);

            if (conflict)
            {
                throw new LessonOrderConflictException(lesson.CourseId, request.Order);
            }

            lesson.Order = request.Order;
        }

        await db.SaveChangesAsync();

        return MapToDetail(lesson);
    }

    public async Task DeleteLessonAsync(Guid id)
    {
        var lesson = await db.Lessons.FirstOrDefaultAsync(l => l.Id == id)
            ?? throw new LessonNotFoundException(id);

        db.Lessons.Remove(lesson);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new CannotDeleteWithAttemptsException("Lesson", id);
        }
    }

    private async Task<Lesson> LoadLessonWithQuestionsAsync(Guid id) =>
        await db.Lessons
            .Include(l => l.Questions.OrderBy(q => q.Order))
            .FirstOrDefaultAsync(l => l.Id == id)
        ?? throw new LessonNotFoundException(id);

    private static AdminLessonDetailDto MapToDetail(Lesson lesson) => new()
    {
        Id = lesson.Id,
        CourseId = lesson.CourseId,
        Order = lesson.Order,
        Title = lesson.Title,
        TheoryMarkdown = lesson.TheoryMarkdown,
        XpReward = lesson.XpReward,
        Questions = lesson.Questions
            .OrderBy(q => q.Order)
            .Select(q => new AdminQuestionSummaryDto
            {
                Id = q.Id,
                Order = q.Order,
                Type = q.Type,
                Text = q.Text
            })
            .ToList()
    };
}
