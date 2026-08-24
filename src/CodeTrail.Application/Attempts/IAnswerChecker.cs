using CodeTrail.Domain.Entities;
using CodeTrail.Domain.Enums;

namespace CodeTrail.Application.Attempts;

// One implementation per QuestionType, registered individually in DI and picked up
// via IAnswerCheckerResolver - no switch-on-type construction (per the architecture spec).
public interface IAnswerChecker
{
    QuestionType QuestionType { get; }

    bool Check(Question question, string givenAnswer);
}
