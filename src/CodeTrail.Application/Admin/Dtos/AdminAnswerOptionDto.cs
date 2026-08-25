namespace CodeTrail.Application.Admin.Dtos;

public class AdminAnswerOptionDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
