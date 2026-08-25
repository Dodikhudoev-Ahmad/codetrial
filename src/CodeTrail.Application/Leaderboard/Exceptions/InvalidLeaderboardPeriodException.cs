using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Leaderboard.Exceptions;

public class InvalidLeaderboardPeriodException(string period)
    : AppException(
        "Invalid period",
        $"Period '{period}' is not valid. Use 'week' or 'all'.",
        HttpStatusCode.BadRequest);
