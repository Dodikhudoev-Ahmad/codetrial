namespace CodeTrail.Domain.Entities;

public class ShortAnswerKey
{
    public Guid QuestionId { get; set; }
    public string ExpectedAnswer { get; set; } = string.Empty;
    public bool IsCaseSensitive { get; set; }

    public Question Question { get; set; } = null!;
}
