using System.ComponentModel.DataAnnotations;
using CodeTrail.Domain.Enums;

namespace CodeTrail.Application.Admin.Dtos;

public class UpsertCourseRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public CourseLevel Level { get; set; }

    [Required]
    [MaxLength(50)]
    public string Language { get; set; } = string.Empty;

    public bool IsPublished { get; set; }
}
