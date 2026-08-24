namespace CodeTrail.Application.Attempts.Dtos;

public class QuestionResultDto
{
    public Guid QuestionId { get; set; }
    public string GivenAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public string Explanation { get; set; } = string.Empty;
    public List<Guid> CorrectOptionIds { get; set; } = [];
    public string? CorrectShortAnswer { get; set; }
}
