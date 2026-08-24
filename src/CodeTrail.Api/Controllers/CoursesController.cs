using CodeTrail.Api.Common;
using CodeTrail.Application.Common;
using CodeTrail.Application.Courses;
using CodeTrail.Application.Courses.Dtos;
using CodeTrail.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeTrail.Api.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<CourseSummaryDto>>> GetCourses([FromQuery] CourseListQuery query) =>
        Ok(await courseService.GetCoursesAsync(query));

    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<CourseDetailDto>> GetCourseBySlug(string slug)
    {
        var userId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : (Guid?)null;
        return Ok(await courseService.GetCourseBySlugAsync(slug, userId));
    }

    [HttpPost("{id:guid}/enroll")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<ActionResult<EnrollmentDto>> Enroll(Guid id) =>
        Ok(await courseService.EnrollAsync(id, User.GetUserId()));
}
