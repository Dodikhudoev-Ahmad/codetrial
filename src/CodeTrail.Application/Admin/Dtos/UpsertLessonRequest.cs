using System.ComponentModel.DataAnnotations;

namespace CodeTrail.Application.Admin.Dtos;

public class UpsertLessonRequest
{
    [Required]
    public Guid CourseId { get; set; }

    // Ignored on create (the lesson is appended after the course's current last
    // lesson); used on update to reorder, with a conflict check against siblings.
    public int Order { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string TheoryMarkdown { get; set; } = string.Empty;

    [Range(0, 1000)]
    public int XpReward { get; set; }

    // Accepts a full YouTube URL (watch/share/shorts/embed/youtu.be) or a bare
    // 11-character video id; parsed and normalized to just the id before storage.
    [MaxLength(500)]
    public string? VideoUrl { get; set; }
}
