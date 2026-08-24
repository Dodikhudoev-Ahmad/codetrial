using System.ComponentModel.DataAnnotations;

namespace CodeTrail.Application.Attempts.Dtos;

public class SubmitAttemptRequest
{
    [Required]
    [MinLength(1)]
    public List<AnswerRequest> Answers { get; set; } = [];
}
