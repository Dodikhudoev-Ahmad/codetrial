using CodeTrail.Domain.Enums;

namespace CodeTrail.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public int TotalXp { get; set; }
    public int CurrentStreak { get; set; }
    public DateOnly? LastActivityDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Course> AuthoredCourses { get; set; } = new List<Course>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<LessonAttempt> LessonAttempts { get; set; } = new List<LessonAttempt>();
}
