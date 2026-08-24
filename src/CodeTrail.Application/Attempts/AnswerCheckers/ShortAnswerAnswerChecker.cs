using CodeTrail.Domain.Entities;
using CodeTrail.Domain.Enums;

namespace CodeTrail.Application.Attempts.AnswerCheckers;

// Business rule 11: compare after trimming and collapsing internal whitespace runs;
// case sensitivity follows the question's ShortAnswerKey.IsCaseSensitive flag.
public class ShortAnswerAnswerChecker : IAnswerChecker
{
    public QuestionType QuestionType => QuestionType.ShortAnswer;

    public bool Check(Question question, string givenAnswer)
    {
        var key = question.ShortAnswerKey
            ?? throw new InvalidOperationException($"Question '{question.Id}' has no ShortAnswerKey configured.");

        var comparison = key.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        return string.Equals(Normalize(givenAnswer), Normalize(key.ExpectedAnswer), comparison);
    }

    private static string Normalize(string value) =>
        string.Join(' ', value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
}
