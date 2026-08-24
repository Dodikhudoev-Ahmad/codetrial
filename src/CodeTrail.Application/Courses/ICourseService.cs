using CodeTrail.Application.Common;
using CodeTrail.Application.Courses.Dtos;

namespace CodeTrail.Application.Courses;

public interface ICourseService
{
    Task<PagedResult<CourseSummaryDto>> GetCoursesAsync(CourseListQuery query);

    Task<CourseDetailDto> GetCourseBySlugAsync(string slug, Guid? currentUserId);

    Task<EnrollmentDto> EnrollAsync(Guid courseId, Guid userId);
}
