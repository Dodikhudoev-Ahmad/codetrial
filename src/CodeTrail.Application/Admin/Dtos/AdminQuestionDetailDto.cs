using CodeTrail.Domain.Enums;

namespace CodeTrail.Application.Admin.Dtos;

public class AdminQuestionDetailDto
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public int Order { get; set; }
    public QuestionType Type { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? CodeSnippet { get; set; }
    public string Explanation { get; set; } = string.Empty;

    // Populated for SingleChoice/MultiChoice; empty for ShortAnswer.
    public List<AdminAnswerOptionDto> Options { get; set; } = [];

    // Populated only for ShortAnswer.
    public string? ExpectedAnswer { get; set; }
    public bool IsCaseSensitive { get; set; }
}
