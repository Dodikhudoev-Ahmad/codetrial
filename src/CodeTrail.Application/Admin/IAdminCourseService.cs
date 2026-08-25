using CodeTrail.Application.Admin.Dtos;
using CodeTrail.Application.Common;

namespace CodeTrail.Application.Admin;

public interface IAdminCourseService
{
    Task<PagedResult<AdminCourseListItemDto>> GetCoursesAsync(int page, int pageSize);

    Task<AdminCourseDetailDto> GetCourseAsync(Guid id);

    Task<AdminCourseDetailDto> CreateCourseAsync(Guid authorId, UpsertCourseRequest request);

    Task<AdminCourseDetailDto> UpdateCourseAsync(Guid id, UpsertCourseRequest request);

    Task DeleteCourseAsync(Guid id);

    Task<CourseStatsDto> GetCourseStatsAsync(Guid id);
}
