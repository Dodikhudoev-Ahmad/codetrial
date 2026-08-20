namespace CodeTrail.Domain.Entities;

public class AnswerSubmission
{
    public Guid Id { get; set; }
    public Guid AttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public string GivenAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }

    public LessonAttempt Attempt { get; set; } = null!;
    public Question Question { get; set; } = null!;
}
