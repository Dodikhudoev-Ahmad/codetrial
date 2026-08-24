using CodeTrail.Domain.Enums;

namespace CodeTrail.Application.Attempts;

public interface IAnswerCheckerResolver
{
    IAnswerChecker Resolve(QuestionType questionType);
}
