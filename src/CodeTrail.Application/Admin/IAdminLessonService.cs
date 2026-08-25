using CodeTrail.Application.Admin.Dtos;

namespace CodeTrail.Application.Admin;

public interface IAdminLessonService
{
    Task<AdminLessonDetailDto> GetLessonAsync(Guid id);

    Task<AdminLessonDetailDto> CreateLessonAsync(UpsertLessonRequest request);

    Task<AdminLessonDetailDto> UpdateLessonAsync(Guid id, UpsertLessonRequest request);

    Task DeleteLessonAsync(Guid id);
}
