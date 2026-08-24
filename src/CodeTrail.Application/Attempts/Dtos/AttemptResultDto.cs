namespace CodeTrail.Application.Attempts.Dtos;

public class AttemptResultDto
{
    public Guid AttemptId { get; set; }
    public int ScorePercent { get; set; }
    public bool IsPassed { get; set; }
    public int AttemptNumber { get; set; }
    public int XpAwarded { get; set; }
    public List<QuestionResultDto> Questions { get; set; } = [];
}
