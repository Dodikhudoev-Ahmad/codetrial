using CodeTrail.Application.Attempts.AnswerCheckers;
using CodeTrail.Domain.Entities;
using CodeTrail.Domain.Enums;

namespace CodeTrail.Tests.Attempts.AnswerCheckers;

public class SingleChoiceAnswerCheckerTests
{
    private readonly SingleChoiceAnswerChecker _checker = new();

    private static Question BuildQuestion(out Guid correctId, out Guid wrongId)
    {
        var question = new Question { Type = QuestionType.SingleChoice, Text = "Q", Explanation = "E" };
        var correct = new AnswerOption { Id = Guid.NewGuid(), Question = question, Text = "Right", IsCorrect = true };
        var wrong = new AnswerOption { Id = Guid.NewGuid(), Question = question, Text = "Wrong", IsCorrect = false };
        question.AnswerOptions.Add(correct);
        question.AnswerOptions.Add(wrong);

        correctId = correct.Id;
        wrongId = wrong.Id;
        return question;
    }

    [Fact]
    public void SelectingTheCorrectOption_IsCorrect()
    {
        var question = BuildQuestion(out var correctId, out _);

        Assert.True(_checker.Check(question, correctId.ToString()));
    }

    [Fact]
    public void SelectingTheWrongOption_IsIncorrect()
    {
        var question = BuildQuestion(out _, out var wrongId);

        Assert.False(_checker.Check(question, wrongId.ToString()));
    }

    [Fact]
    public void MalformedAnswer_IsIncorrectRatherThanThrowing()
    {
        var question = BuildQuestion(out _, out _);

        Assert.False(_checker.Check(question, "not-a-guid"));
    }
}
