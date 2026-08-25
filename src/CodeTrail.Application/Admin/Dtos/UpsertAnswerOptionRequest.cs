using System.ComponentModel.DataAnnotations;

namespace CodeTrail.Application.Admin.Dtos;

public class UpsertAnswerOptionRequest
{
    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}
