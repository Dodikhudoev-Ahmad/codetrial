using System.ComponentModel.DataAnnotations;
using CodeTrail.Domain.Enums;

namespace CodeTrail.Application.Courses.Dtos;

public class CourseListQuery
{
    public CourseLevel? Level { get; set; }

    public string? Language { get; set; }

    public string? Search { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1.")]
    public int Page { get; set; } = 1;

    [Range(1, 50, ErrorMessage = "PageSize must be between 1 and 50.")]
    public int PageSize { get; set; } = 10;
}
