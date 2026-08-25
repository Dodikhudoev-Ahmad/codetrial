using CodeTrail.Application.Profile.Dtos;

namespace CodeTrail.Application.Profile;

public interface IProfileService
{
    Task<List<CourseProgressDto>> GetProgressAsync(Guid userId);
}
