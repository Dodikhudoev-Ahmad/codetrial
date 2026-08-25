namespace CodeTrail.Application.Admin.Dtos;

public class CourseStatsDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int EnrollmentsCount { get; set; }
    public int CompletionsCount { get; set; }
    public double AverageScorePercent { get; set; }
    public List<LessonStatsDto> Lessons { get; set; } = [];
}

public class LessonStatsDto
{
    public Guid LessonId { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public int AttemptsCount { get; set; }
    public int StudentsPassedCount { get; set; }
    public double AverageScorePercent { get; set; }
}
