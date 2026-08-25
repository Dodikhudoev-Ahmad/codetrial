using System.ComponentModel.DataAnnotations;
using CodeTrail.Domain.Enums;

namespace CodeTrail.Application.Admin.Dtos;

public class UpsertQuestionRequest
{
    [Required]
    public Guid LessonId { get; set; }

    public QuestionType Type { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Text { get; set; } = string.Empty;

    public string? CodeSnippet { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Explanation { get; set; } = string.Empty;

    // Required (with business-rule validation on counts/correctness) for
    // SingleChoice/MultiChoice; ignored for ShortAnswer.
    public List<UpsertAnswerOptionRequest> Options { get; set; } = [];

    // Required for ShortAnswer; ignored otherwise.
    [MaxLength(500)]
    public string? ExpectedAnswer { get; set; }

    public bool IsCaseSensitive { get; set; }
}
