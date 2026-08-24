using CodeTrail.Api.Common;
using CodeTrail.Application.Lessons;
using CodeTrail.Application.Lessons.Dtos;
using CodeTrail.Application.Lessons.Exceptions;
using CodeTrail.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeTrail.Api.Controllers;

[ApiController]
[Route("api/lessons")]
public class LessonsController(ILessonService lessonService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<ActionResult<LessonDetailDto>> GetLesson(Guid id)
    {
        try
        {
            return Ok(await lessonService.GetLessonAsync(id, User.GetUserId()));
        }
        catch (LessonNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Lesson not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (NotEnrolledException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
            {
                Title = "Not enrolled",
                Detail = ex.Message,
                Status = StatusCodes.Status403Forbidden
            });
        }
        catch (LessonLockedException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
            {
                Title = "Lesson locked",
                Detail = ex.Message,
                Status = StatusCodes.Status403Forbidden
            });
        }
    }
}
