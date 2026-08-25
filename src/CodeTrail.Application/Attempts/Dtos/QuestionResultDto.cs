using CodeTrail.Domain.Enums;

namespace CodeTrail.Application.Attempts.Dtos;

public class QuestionResultDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public string? CodeSnippet { get; set; }
    public string GivenAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public string Explanation { get; set; } = string.Empty;

    // Empty for ShortAnswer questions - use CorrectShortAnswer instead.
    public List<AnswerOptionResultDto> Options { get; set; } = [];
    public string? CorrectShortAnswer { get; set; }
}
