using CodeTrail.Application.Attempts.Dtos;

namespace CodeTrail.Application.Attempts;

public interface IAttemptService
{
    Task<AttemptResultDto> SubmitAttemptAsync(Guid lessonId, Guid userId, SubmitAttemptRequest request);

    Task<AttemptResultDto> GetAttemptAsync(Guid attemptId, Guid userId);
}
