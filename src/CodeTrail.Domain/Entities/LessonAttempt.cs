namespace CodeTrail.Domain.Entities;

public class LessonAttempt
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid LessonId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public int ScorePercent { get; set; }
    public bool IsPassed { get; set; }
    public int AttemptNumber { get; set; }

    public User User { get; set; } = null!;
    public Lesson Lesson { get; set; } = null!;
    public ICollection<AnswerSubmission> AnswerSubmissions { get; set; } = new List<AnswerSubmission>();
}
