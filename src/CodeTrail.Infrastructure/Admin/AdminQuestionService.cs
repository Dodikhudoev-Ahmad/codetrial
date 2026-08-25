using CodeTrail.Application.Admin;
using CodeTrail.Application.Admin.Dtos;
using CodeTrail.Application.Admin.Exceptions;
using CodeTrail.Application.Lessons.Exceptions;
using CodeTrail.Domain.Entities;
using CodeTrail.Domain.Enums;
using CodeTrail.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeTrail.Infrastructure.Admin;

public class AdminQuestionService(CodeTrailDbContext db) : IAdminQuestionService
{
    public async Task<AdminQuestionDetailDto> GetQuestionAsync(Guid id)
    {
        var question = await LoadQuestionAsync(id);
        return MapToDetail(question);
    }

    public async Task<AdminQuestionDetailDto> CreateQuestionAsync(UpsertQuestionRequest request)
    {
        var lessonExists = await db.Lessons.AnyAsync(l => l.Id == request.LessonId);

        if (!lessonExists)
        {
            throw new LessonNotFoundException(request.LessonId);
        }

        ValidateDefinition(request);

        var maxOrder = await db.Questions
            .Where(q => q.LessonId == request.LessonId)
            .Select(q => (int?)q.Order)
            .MaxAsync() ?? 0;

        var question = new Question
        {
            LessonId = request.LessonId,
            Order = maxOrder + 1,
            Type = request.Type,
            Text = request.Text.Trim(),
            CodeSnippet = string.IsNullOrWhiteSpace(request.CodeSnippet) ? null : request.CodeSnippet,
            Explanation = request.Explanation.Trim()
        };

        ApplyAnswerDefinition(question, request);

        db.Questions.Add(question);
        await db.SaveChangesAsync();

        return MapToDetail(question);
    }

    public async Task<AdminQuestionDetailDto> UpdateQuestionAsync(Guid id, UpsertQuestionRequest request)
    {
        var question = await LoadQuestionAsync(id);

        ValidateDefinition(request);

        question.Type = request.Type;
        question.Text = request.Text.Trim();
        question.CodeSnippet = string.IsNullOrWhiteSpace(request.CodeSnippet) ? null : request.CodeSnippet;
        question.Explanation = request.Explanation.Trim();

        // Replace children wholesale rather than diffing - simpler and correct for an
        // admin editing tool at this project's scale, and the client never has a reason
        // to keep a specific AnswerOption's id stable across an edit.
        db.AnswerOptions.RemoveRange(question.AnswerOptions);
        question.AnswerOptions.Clear();

        if (question.ShortAnswerKey is not null)
        {
            db.ShortAnswerKeys.Remove(question.ShortAnswerKey);
            question.ShortAnswerKey = null;
        }

        ApplyAnswerDefinition(question, request);

        await db.SaveChangesAsync();

        return MapToDetail(question);
    }

    public async Task DeleteQuestionAsync(Guid id)
    {
        var question = await db.Questions.FirstOrDefaultAsync(q => q.Id == id)
            ?? throw new QuestionNotFoundException(id);

        db.Questions.Remove(question);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new CannotDeleteWithAttemptsException("Question", id);
        }
    }

    private async Task<Question> LoadQuestionAsync(Guid id) =>
        await db.Questions
            .Include(q => q.AnswerOptions)
            .Include(q => q.ShortAnswerKey)
            .FirstOrDefaultAsync(q => q.Id == id)
        ?? throw new QuestionNotFoundException(id);

    private static void ValidateDefinition(UpsertQuestionRequest request)
    {
        switch (request.Type)
        {
            case QuestionType.SingleChoice:
                if (request.Options.Count < 2)
                {
                    throw new InvalidQuestionDefinitionException("A single-choice question needs at least two options.");
                }
                if (request.Options.Count(o => o.IsCorrect) != 1)
                {
                    throw new InvalidQuestionDefinitionException("A single-choice question needs exactly one correct option.");
                }
                break;

            case QuestionType.MultiChoice:
                if (request.Options.Count < 2)
                {
                    throw new InvalidQuestionDefinitionException("A multi-choice question needs at least two options.");
                }
                if (!request.Options.Any(o => o.IsCorrect))
                {
                    throw new InvalidQuestionDefinitionException("A multi-choice question needs at least one correct option.");
                }
                break;

            case QuestionType.ShortAnswer:
                if (string.IsNullOrWhiteSpace(request.ExpectedAnswer))
                {
                    throw new InvalidQuestionDefinitionException("A short-answer question needs an expected answer.");
                }
                break;
        }
    }

    private static void ApplyAnswerDefinition(Question question, UpsertQuestionRequest request)
    {
        if (request.Type == QuestionType.ShortAnswer)
        {
            question.ShortAnswerKey = new ShortAnswerKey
            {
                Question = question,
                ExpectedAnswer = request.ExpectedAnswer!.Trim(),
                IsCaseSensitive = request.IsCaseSensitive
            };
            return;
        }

        foreach (var option in request.Options)
        {
            question.AnswerOptions.Add(new AnswerOption
            {
                Question = question,
                Text = option.Text.Trim(),
                IsCorrect = option.IsCorrect
            });
        }
    }

    private static AdminQuestionDetailDto MapToDetail(Question question) => new()
    {
        Id = question.Id,
        LessonId = question.LessonId,
        Order = question.Order,
        Type = question.Type,
        Text = question.Text,
        CodeSnippet = question.CodeSnippet,
        Explanation = question.Explanation,
        Options = question.AnswerOptions
            .Select(o => new AdminAnswerOptionDto { Id = o.Id, Text = o.Text, IsCorrect = o.IsCorrect })
            .ToList(),
        ExpectedAnswer = question.ShortAnswerKey?.ExpectedAnswer,
        IsCaseSensitive = question.ShortAnswerKey?.IsCaseSensitive ?? false
    };
}
