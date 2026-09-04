namespace CodeTrail.Application.Admin.Dtos;

public class AdminLessonDetailDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TheoryMarkdown { get; set; } = string.Empty;
    public int XpReward { get; set; }
    public string? YouTubeVideoId { get; set; }
    public List<AdminQuestionSummaryDto> Questions { get; set; } = [];
}
