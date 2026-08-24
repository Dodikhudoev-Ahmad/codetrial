using CodeTrail.Domain.Enums;

namespace CodeTrail.Application.Lessons.Dtos;

// Deliberately has no Explanation and no ShortAnswerKey: both would reveal the correct
// answer before the student submits (business rule 7). Explanation is only returned
// as part of the attempt result once the answer has been checked.
public class QuestionPreviewDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public QuestionType Type { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? CodeSnippet { get; set; }
    public List<AnswerOptionPreviewDto> Options { get; set; } = [];
}
