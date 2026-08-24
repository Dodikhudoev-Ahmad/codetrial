using CodeTrail.Domain.Entities;
using CodeTrail.Domain.Enums;

namespace CodeTrail.Application.Attempts.AnswerCheckers;

// GivenAnswer is the id of the selected AnswerOption.
public class SingleChoiceAnswerChecker : IAnswerChecker
{
    public QuestionType QuestionType => QuestionType.SingleChoice;

    public bool Check(Question question, string givenAnswer)
    {
        if (!Guid.TryParse(givenAnswer.Trim(), out var selectedId))
        {
            return false;
        }

        var correctOption = question.AnswerOptions.FirstOrDefault(o => o.IsCorrect);
        return correctOption is not null && correctOption.Id == selectedId;
    }
}
