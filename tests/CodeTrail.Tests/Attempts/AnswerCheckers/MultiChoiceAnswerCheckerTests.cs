using CodeTrail.Application.Attempts.AnswerCheckers;
using CodeTrail.Domain.Entities;
using CodeTrail.Domain.Enums;

namespace CodeTrail.Tests.Attempts.AnswerCheckers;

public class MultiChoiceAnswerCheckerTests
{
    private readonly MultiChoiceAnswerChecker _checker = new();

    private static Question BuildQuestion(out Guid correctA, out Guid correctB, out Guid wrong)
    {
        var question = new Question { Type = QuestionType.MultiChoice, Text = "Q", Explanation = "E" };
        var a = new AnswerOption { Id = Guid.NewGuid(), Question = question, Text = "A", IsCorrect = true };
        var b = new AnswerOption { Id = Guid.NewGuid(), Question = question, Text = "B", IsCorrect = true };
        var c = new AnswerOption { Id = Guid.NewGuid(), Question = question, Text = "C", IsCorrect = false };
        question.AnswerOptions.Add(a);
        question.AnswerOptions.Add(b);
        question.AnswerOptions.Add(c);

        correctA = a.Id;
        correctB = b.Id;
        wrong = c.Id;
        return question;
    }

    [Fact]
    public void ExactCorrectSet_IsCorrect()
    {
        var question = BuildQuestion(out var a, out var b, out _);

        Assert.True(_checker.Check(question, $"{a},{b}"));
    }

    [Fact]
    public void ExactCorrectSet_OrderDoesNotMatter()
    {
        var question = BuildQuestion(out var a, out var b, out _);

        Assert.True(_checker.Check(question, $"{b},{a}"));
    }

    [Fact]
    public void MissingOneCorrectOption_IsIncorrect()
    {
        var question = BuildQuestion(out var a, out _, out _);

        Assert.False(_checker.Check(question, a.ToString()));
    }

    [Fact]
    public void IncludingAWrongOption_IsIncorrect()
    {
        var question = BuildQuestion(out var a, out var b, out var wrong);

        Assert.False(_checker.Check(question, $"{a},{b},{wrong}"));
    }
}
