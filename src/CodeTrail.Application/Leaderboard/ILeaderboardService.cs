using CodeTrail.Application.Leaderboard.Dtos;

namespace CodeTrail.Application.Leaderboard;

public interface ILeaderboardService
{
    Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(LeaderboardPeriod period);
}
