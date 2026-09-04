using System.Net;
using CodeTrail.Application.Common.Exceptions;
using CodeTrail.Application.Lessons;

namespace CodeTrail.Application.Attempts.Exceptions;

public class VideoNotWatchedException(Guid lessonId, int watchedPercent)
    : AppException(
        "Video not watched",
        $"Lesson '{lessonId}' requires watching at least {VideoProgressRules.RequiredWatchPercent}% of its " +
        $"video before an attempt can be submitted (currently at {watchedPercent}%).",
        HttpStatusCode.Forbidden);
