namespace CodeTrail.Application.Lessons.Dtos;

// Deliberately has no IsCorrect field: GET /api/lessons/{id} must never leak which
// option is right (business rule 7).
public class AnswerOptionPreviewDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
}
