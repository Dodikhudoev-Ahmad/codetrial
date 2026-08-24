using System.ComponentModel.DataAnnotations;

namespace CodeTrail.Application.Attempts.Dtos;

public class AnswerRequest
{
    [Required]
    public Guid QuestionId { get; set; }

    public string GivenAnswer { get; set; } = string.Empty;
}
