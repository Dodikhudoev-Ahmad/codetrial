using CodeTrail.Api.Common;
using CodeTrail.Application.Attempts;
using CodeTrail.Application.Attempts.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeTrail.Api.Controllers;

[ApiController]
[Route("api/attempts")]
public class AttemptsController(IAttemptService attemptService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<AttemptResultDto>> GetAttempt(Guid id) =>
        Ok(await attemptService.GetAttemptAsync(id, User.GetUserId()));
}
