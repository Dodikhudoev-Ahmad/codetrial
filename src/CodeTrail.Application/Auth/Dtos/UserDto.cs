namespace CodeTrail.Application.Auth.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int TotalXp { get; set; }
    public int CurrentStreak { get; set; }
    public DateOnly? LastActivityDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
