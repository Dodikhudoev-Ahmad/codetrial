using CodeTrail.Application.Attempts.AnswerCheckers;
using CodeTrail.Domain.Entities;
using CodeTrail.Domain.Enums;

namespace CodeTrail.Tests.Attempts.AnswerCheckers;

public class ShortAnswerAnswerCheckerTests
{
    private readonly ShortAnswerAnswerChecker _checker = new();

    private static Question BuildQuestion(string expectedAnswer, bool isCaseSensitive)
    {
        var question = new Question { Type = QuestionType.ShortAnswer, Text = "Q", Explanation = "E" };
        question.ShortAnswerKey = new ShortAnswerKey
        {
            Question = question,
            ExpectedAnswer = expectedAnswer,
            IsCaseSensitive = isCaseSensitive
        };
        return question;
    }

    [Fact]
    public void ExactMatch_IsCorrect()
    {
        var question = BuildQuestion("var", isCaseSensitive: false);

        Assert.True(_checker.Check(question, "var"));
    }

    [Fact]
    public void CaseInsensitiveByDefault_DifferentCaseStillCorrect()
    {
        var question = BuildQuestion("var", isCaseSensitive: false);

        Assert.True(_checker.Check(question, "VAR"));
    }

    [Fact]
    public void CaseSensitive_DifferentCaseIsIncorrect()
    {
        var question = BuildQuestion("var", isCaseSensitive: true);

        Assert.False(_checker.Check(question, "VAR"));
    }

    [Theory]
    [InlineData("  var  ")]
    [InlineData("var\t")]
    public void LeadingAndTrailingWhitespace_IsTrimmed(string givenAnswer)
    {
        var question = BuildQuestion("var", isCaseSensitive: false);

        Assert.True(_checker.Check(question, givenAnswer));
    }

    [Fact]
    public void InternalWhitespaceRuns_AreNormalizedToSingleSpace()
    {
        var question = BuildQuestion("Hello world", isCaseSensitive: false);

        Assert.True(_checker.Check(question, "Hello   world"));
    }

    [Fact]
    public void WrongAnswer_IsIncorrect()
    {
        var question = BuildQuestion("var", isCaseSensitive: false);

        Assert.False(_checker.Check(question, "int"));
    }
}
