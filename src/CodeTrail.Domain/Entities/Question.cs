using CodeTrail.Domain.Enums;

namespace CodeTrail.Domain.Entities;

public class Question
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public int Order { get; set; }
    public QuestionType Type { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? CodeSnippet { get; set; }
    public string Explanation { get; set; } = string.Empty;

    public Lesson Lesson { get; set; } = null!;
    public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
    public ShortAnswerKey? ShortAnswerKey { get; set; }
    public ICollection<AnswerSubmission> AnswerSubmissions { get; set; } = new List<AnswerSubmission>();
}
