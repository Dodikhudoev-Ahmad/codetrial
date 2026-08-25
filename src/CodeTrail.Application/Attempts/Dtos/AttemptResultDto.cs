namespace CodeTrail.Application.Attempts.Dtos;

public class AttemptResultDto
{
    public Guid AttemptId { get; set; }
    public Guid LessonId { get; set; }
    public string CourseSlug { get; set; } = string.Empty;
    public Guid? NextLessonId { get; set; }
    public int ScorePercent { get; set; }
    public bool IsPassed { get; set; }
    public int AttemptNumber { get; set; }
    public int XpAwarded { get; set; }
    public List<QuestionResultDto> Questions { get; set; } = [];
}
