using CodeTrail.Api.Common;
using CodeTrail.Application.Profile;
using CodeTrail.Application.Profile.Dtos;
using CodeTrail.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeTrail.Api.Controllers;

[ApiController]
[Route("api/me")]
public class MeController(IProfileService profileService) : ControllerBase
{
    [HttpGet("progress")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<ActionResult<List<CourseProgressDto>>> GetProgress() =>
        Ok(await profileService.GetProgressAsync(User.GetUserId()));
}
