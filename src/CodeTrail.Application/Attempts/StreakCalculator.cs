namespace CodeTrail.Application.Attempts;

// Business rule 8: +1 if the previous activity was yesterday, reset to 1 if a day was
// skipped (or this is the very first activity ever), unchanged if already active today.
public static class StreakCalculator
{
    public static (int CurrentStreak, DateOnly LastActivityDate) Apply(
        int currentStreak, DateOnly? lastActivityDate, DateOnly today)
    {
        if (lastActivityDate == today)
        {
            return (currentStreak, today);
        }

        var newStreak = lastActivityDate == today.AddDays(-1) ? currentStreak + 1 : 1;
        return (newStreak, today);
    }
}
