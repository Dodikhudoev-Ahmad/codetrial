using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Admin.Exceptions;

public class InvalidYouTubeUrlException(string input)
    : AppException(
        "Invalid YouTube link",
        $"'{input}' is not a recognizable YouTube video URL or id.",
        HttpStatusCode.BadRequest);
