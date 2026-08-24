using CodeTrail.Api.Common;
using CodeTrail.Application.Common;
using CodeTrail.Application.Courses;
using CodeTrail.Application.Courses.Dtos;
using CodeTrail.Application.Courses.Exceptions;
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
    public async Task<ActionResult<PagedResult<CourseSummaryDto>>> GetCourses([FromQuery] CourseListQuery query)
    {
        return Ok(await courseService.GetCoursesAsync(query));
    }

    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<CourseDetailDto>> GetCourseBySlug(string slug)
    {
        try
        {
            var userId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : (Guid?)null;
            return Ok(await courseService.GetCourseBySlugAsync(slug, userId));
        }
        catch (CourseNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Course not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
    }

    [HttpPost("{id:guid}/enroll")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<ActionResult<EnrollmentDto>> Enroll(Guid id)
    {
        try
        {
            return Ok(await courseService.EnrollAsync(id, User.GetUserId()));
        }
        catch (CourseNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Course not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (AlreadyEnrolledException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Already enrolled",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }
}
