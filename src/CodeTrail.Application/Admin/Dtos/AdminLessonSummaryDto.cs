namespace CodeTrail.Application.Admin.Dtos;

public class AdminLessonSummaryDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public int XpReward { get; set; }
    public int QuestionsCount { get; set; }
}
