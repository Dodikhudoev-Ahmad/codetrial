namespace CodeTrail.Application.Leaderboard.Dtos;

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int Xp { get; set; }
}
