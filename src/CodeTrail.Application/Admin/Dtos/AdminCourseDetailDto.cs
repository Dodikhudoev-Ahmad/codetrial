using CodeTrail.Domain.Enums;

namespace CodeTrail.Application.Admin.Dtos;

public class AdminCourseDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CourseLevel Level { get; set; }
    public string Language { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public List<AdminLessonSummaryDto> Lessons { get; set; } = [];
}
