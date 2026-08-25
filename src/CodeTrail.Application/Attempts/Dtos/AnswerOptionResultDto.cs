namespace CodeTrail.Application.Attempts.Dtos;

public class AnswerOptionResultDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
