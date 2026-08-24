using CodeTrail.Application.Lessons.Dtos;

namespace CodeTrail.Application.Lessons;

public interface ILessonService
{
    Task<LessonDetailDto> GetLessonAsync(Guid lessonId, Guid userId);
}
