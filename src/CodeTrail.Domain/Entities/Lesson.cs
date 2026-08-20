namespace CodeTrail.Domain.Entities;

public class Lesson
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TheoryMarkdown { get; set; } = string.Empty;
    public int XpReward { get; set; }

    public Course Course { get; set; } = null!;
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<LessonAttempt> Attempts { get; set; } = new List<LessonAttempt>();
}
