using CodeTrail.Domain.Enums;

namespace CodeTrail.Application.Courses.Dtos;

public class CourseDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CourseLevel Level { get; set; }
    public string Language { get; set; } = string.Empty;
    public bool IsEnrolled { get; set; }
    public List<LessonSummaryDto> Lessons { get; set; } = [];
}
