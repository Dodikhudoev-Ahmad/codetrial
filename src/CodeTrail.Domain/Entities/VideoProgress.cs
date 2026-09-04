namespace CodeTrail.Domain.Entities;

public class VideoProgress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid LessonId { get; set; }
    public int WatchedPercent { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public Lesson Lesson { get; set; } = null!;
}
