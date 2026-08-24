using CodeTrail.Application.Attempts;

namespace CodeTrail.Tests.Attempts;

public class StreakCalculatorTests
{
    private static readonly DateOnly Today = new(2026, 8, 24);

    [Fact]
    public void ActivityYesterday_IncrementsStreak()
    {
        var (streak, lastActivityDate) = StreakCalculator.Apply(
            currentStreak: 4, lastActivityDate: Today.AddDays(-1), today: Today);

        Assert.Equal(5, streak);
        Assert.Equal(Today, lastActivityDate);
    }

    [Fact]
    public void ActivityTwoDaysAgo_ResetsStreakToOne()
    {
        var (streak, lastActivityDate) = StreakCalculator.Apply(
            currentStreak: 10, lastActivityDate: Today.AddDays(-2), today: Today);

        Assert.Equal(1, streak);
        Assert.Equal(Today, lastActivityDate);
    }

    [Fact]
    public void NoPriorActivity_StartsStreakAtOne()
    {
        var (streak, lastActivityDate) = StreakCalculator.Apply(
            currentStreak: 0, lastActivityDate: null, today: Today);

        Assert.Equal(1, streak);
        Assert.Equal(Today, lastActivityDate);
    }

    [Fact]
    public void RepeatActivityOnSameDay_LeavesStreakUnchanged()
    {
        var (streak, lastActivityDate) = StreakCalculator.Apply(
            currentStreak: 7, lastActivityDate: Today, today: Today);

        Assert.Equal(7, streak);
        Assert.Equal(Today, lastActivityDate);
    }
}
