using CodeTrail.Domain.Enums;

namespace CodeTrail.Domain.Entities;

public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CourseLevel Level { get; set; }
    public string Language { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public Guid AuthorId { get; set; }

    public User Author { get; set; } = null!;
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
