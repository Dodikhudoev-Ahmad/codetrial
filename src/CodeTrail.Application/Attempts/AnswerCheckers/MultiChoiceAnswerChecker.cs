using CodeTrail.Domain.Entities;
using CodeTrail.Domain.Enums;

namespace CodeTrail.Application.Attempts.AnswerCheckers;

// GivenAnswer is a comma-separated list of selected AnswerOption ids; correct only
// if the selected set is exactly the set of options marked IsCorrect.
public class MultiChoiceAnswerChecker : IAnswerChecker
{
    public QuestionType QuestionType => QuestionType.MultiChoice;

    public bool Check(Question question, string givenAnswer)
    {
        var selectedIds = ParseIds(givenAnswer);
        var correctIds = question.AnswerOptions.Where(o => o.IsCorrect).Select(o => o.Id).ToHashSet();

        return selectedIds.SetEquals(correctIds);
    }

    private static HashSet<Guid> ParseIds(string raw) =>
        raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(part => Guid.TryParse(part, out _))
            .Select(Guid.Parse)
            .ToHashSet();
}
