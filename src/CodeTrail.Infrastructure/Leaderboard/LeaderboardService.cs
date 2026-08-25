using CodeTrail.Application.Leaderboard;
using CodeTrail.Application.Leaderboard.Dtos;
using CodeTrail.Domain.Enums;
using CodeTrail.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeTrail.Infrastructure.Leaderboard;

public class LeaderboardService(CodeTrailDbContext db) : ILeaderboardService
{
    private const int TopCount = 20;

    public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(LeaderboardPeriod period) =>
        period == LeaderboardPeriod.All
            ? await GetAllTimeAsync()
            : await GetWeeklyAsync();

    private async Task<List<LeaderboardEntryDto>> GetAllTimeAsync()
    {
        var top = await db.Users
            .Where(u => u.Role == UserRole.Student)
            .OrderByDescending(u => u.TotalXp)
            .Take(TopCount)
            .Select(u => new { u.Id, u.DisplayName, u.TotalXp })
            .ToListAsync();

        return top
            .Select((u, index) => new LeaderboardEntryDto
            {
                Rank = index + 1,
                UserId = u.Id,
                DisplayName = u.DisplayName,
                Xp = u.TotalXp
            })
            .ToList();
    }

    // XP awarded per attempt isn't persisted anywhere (only the running User.TotalXp
    // total), so "XP earned this week" is reconstructed from business rule 4: XP counts
    // once, at a lesson's first passing attempt. Small dataset for this project's scope,
    // so the grouping is done in memory rather than fighting EF's SQL translation of a
    // GroupBy-then-Min-then-GroupBy-then-Sum pipeline.
    private async Task<List<LeaderboardEntryDto>> GetWeeklyAsync()
    {
        var since = DateTime.UtcNow.AddDays(-7);

        var passingAttempts = await db.LessonAttempts
            .Where(a => a.IsPassed)
            .Select(a => new { a.UserId, a.LessonId, a.StartedAt })
            .ToListAsync();

        var lessonXpById = await db.Lessons.ToDictionaryAsync(l => l.Id, l => l.XpReward);

        var weeklyXpByUser = passingAttempts
            .GroupBy(a => new { a.UserId, a.LessonId })
            .Select(g => new { g.Key.UserId, g.Key.LessonId, FirstPassedAt = g.Min(a => a.StartedAt) })
            .Where(x => x.FirstPassedAt >= since)
            .GroupBy(x => x.UserId)
            .Select(g => new { UserId = g.Key, Xp = g.Sum(x => lessonXpById.GetValueOrDefault(x.LessonId)) })
            .Where(x => x.Xp > 0)
            .OrderByDescending(x => x.Xp)
            .Take(TopCount)
            .ToList();

        var userIds = weeklyXpByUser.Select(x => x.UserId).ToList();
        var displayNames = await db.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.DisplayName);

        return weeklyXpByUser
            .Select((x, index) => new LeaderboardEntryDto
            {
                Rank = index + 1,
                UserId = x.UserId,
                DisplayName = displayNames.GetValueOrDefault(x.UserId, "—"),
                Xp = x.Xp
            })
            .ToList();
    }
}
