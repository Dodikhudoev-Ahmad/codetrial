namespace CodeTrail.Application.Profile.Dtos;

public class CourseProgressDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string CourseSlug { get; set; } = string.Empty;
    public int TotalLessons { get; set; }
    public int PassedLessons { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
