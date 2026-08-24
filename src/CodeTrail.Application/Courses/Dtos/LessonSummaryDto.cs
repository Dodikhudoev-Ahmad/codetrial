namespace CodeTrail.Application.Courses.Dtos;

public class LessonSummaryDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public int XpReward { get; set; }
    public LessonStatus Status { get; set; }
}
