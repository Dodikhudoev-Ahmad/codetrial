using System.Text.RegularExpressions;

namespace CodeTrail.Application.Common;

// Accepts anything an admin might paste - a full watch/share/shorts/embed URL, a
// youtu.be short link, or a bare 11-character id - and reduces it to just the id.
// Storing only the id (never the raw URL) means the player always renders through
// our own youtube-nocookie.com embed, so pasted input can't smuggle in an
// arbitrary iframe target.
public static partial class YouTubeUrlParser
{
    [GeneratedRegex(@"^[A-Za-z0-9_-]{11}$")]
    private static partial Regex BareIdPattern();

    [GeneratedRegex(@"(?:youtube(?:-nocookie)?\.com/(?:watch\?v=|embed/|shorts/)|youtu\.be/)([A-Za-z0-9_-]{11})")]
    private static partial Regex UrlPattern();

    public static bool TryExtractVideoId(string? input, out string? videoId)
    {
        videoId = null;

        if (string.IsNullOrWhiteSpace(input))
        {
            return true; // Absent video is valid - it's an optional field.
        }

        var trimmed = input.Trim();

        if (BareIdPattern().IsMatch(trimmed))
        {
            videoId = trimmed;
            return true;
        }

        var match = UrlPattern().Match(trimmed);
        if (match.Success)
        {
            videoId = match.Groups[1].Value;
            return true;
        }

        return false;
    }
}
