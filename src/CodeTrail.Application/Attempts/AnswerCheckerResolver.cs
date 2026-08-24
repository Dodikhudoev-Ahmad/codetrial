using CodeTrail.Domain.Enums;

namespace CodeTrail.Application.Attempts;

public class AnswerCheckerResolver : IAnswerCheckerResolver
{
    private readonly Dictionary<QuestionType, IAnswerChecker> _checkersByType;

    public AnswerCheckerResolver(IEnumerable<IAnswerChecker> checkers)
    {
        _checkersByType = checkers.ToDictionary(c => c.QuestionType);
    }

    public IAnswerChecker Resolve(QuestionType questionType) =>
        _checkersByType.TryGetValue(questionType, out var checker)
            ? checker
            : throw new InvalidOperationException($"No IAnswerChecker is registered for question type '{questionType}'.");
}
