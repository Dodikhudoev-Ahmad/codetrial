using CodeTrail.Domain.Enums;

namespace CodeTrail.Application.Admin.Dtos;

public class AdminQuestionSummaryDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public QuestionType Type { get; set; }
    public string Text { get; set; } = string.Empty;
}
