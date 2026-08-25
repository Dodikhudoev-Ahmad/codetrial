using CodeTrail.Application.Leaderboard;
using CodeTrail.Application.Leaderboard.Dtos;
using CodeTrail.Application.Leaderboard.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeTrail.Api.Controllers;

[ApiController]
[Route("api/leaderboard")]
public class LeaderboardController(ILeaderboardService leaderboardService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<LeaderboardEntryDto>>> GetLeaderboard([FromQuery] string period = "all")
    {
        if (!Enum.TryParse<LeaderboardPeriod>(period, ignoreCase: true, out var parsedPeriod))
        {
            throw new InvalidLeaderboardPeriodException(period);
        }

        return Ok(await leaderboardService.GetLeaderboardAsync(parsedPeriod));
    }
}
