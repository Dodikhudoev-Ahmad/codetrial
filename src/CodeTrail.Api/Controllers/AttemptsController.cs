using CodeTrail.Api.Common;
using CodeTrail.Application.Attempts;
using CodeTrail.Application.Attempts.Dtos;
using CodeTrail.Application.Attempts.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeTrail.Api.Controllers;

[ApiController]
[Route("api/attempts")]
public class AttemptsController(IAttemptService attemptService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<AttemptResultDto>> GetAttempt(Guid id)
    {
        try
        {
            return Ok(await attemptService.GetAttemptAsync(id, User.GetUserId()));
        }
        catch (AttemptNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Attempt not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (AttemptAccessDeniedException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
            {
                Title = "Access denied",
                Detail = ex.Message,
                Status = StatusCodes.Status403Forbidden
            });
        }
    }
}
