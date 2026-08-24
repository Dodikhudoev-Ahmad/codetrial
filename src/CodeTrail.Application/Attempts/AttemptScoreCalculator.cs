namespace CodeTrail.Application.Attempts;

// Business rule 2: a lesson counts as passed at 70% or higher.
public static class AttemptScoreCalculator
{
    public const int PassingScorePercent = 70;

    public static int CalculateScorePercent(int correctCount, int totalCount) =>
        (int)Math.Round(correctCount * 100.0 / totalCount, MidpointRounding.AwayFromZero);

    public static bool IsPassing(int scorePercent) => scorePercent >= PassingScorePercent;
}
