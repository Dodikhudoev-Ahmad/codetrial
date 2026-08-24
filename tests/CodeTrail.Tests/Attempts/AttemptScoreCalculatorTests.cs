using CodeTrail.Application.Attempts;

namespace CodeTrail.Tests.Attempts;

public class AttemptScoreCalculatorTests
{
    // Business rule 2: the pass threshold is 70%. These are the exact boundary cases
    // the spec calls out: 69 must fail, 70 and 71 must pass.
    [Theory]
    [InlineData(69, 100, false)]
    [InlineData(70, 100, true)]
    [InlineData(71, 100, true)]
    public void IsPassing_RespectsThreshold(int correctCount, int totalCount, bool expectedPassed)
    {
        var scorePercent = AttemptScoreCalculator.CalculateScorePercent(correctCount, totalCount);

        Assert.Equal(correctCount, scorePercent);
        Assert.Equal(expectedPassed, AttemptScoreCalculator.IsPassing(scorePercent));
    }

    [Theory]
    [InlineData(1, 3, 33)]
    [InlineData(2, 3, 67)]
    [InlineData(3, 3, 100)]
    public void CalculateScorePercent_RoundsToNearestWholePercent(int correctCount, int totalCount, int expectedPercent)
    {
        Assert.Equal(expectedPercent, AttemptScoreCalculator.CalculateScorePercent(correctCount, totalCount));
    }
}
