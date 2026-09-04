using CodeTrail.Application.Common;

namespace CodeTrail.Tests.Common;

public class YouTubeUrlParserTests
{
    [Fact]
    public void NullOrEmpty_IsValidWithNoVideoId()
    {
        Assert.True(YouTubeUrlParser.TryExtractVideoId(null, out var id));
        Assert.Null(id);

        Assert.True(YouTubeUrlParser.TryExtractVideoId("   ", out id));
        Assert.Null(id);
    }

    [Theory]
    [InlineData("dQw4w9WgXcQ")]
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ&t=42s")]
    [InlineData("https://youtu.be/dQw4w9WgXcQ")]
    [InlineData("https://www.youtube.com/embed/dQw4w9WgXcQ")]
    [InlineData("https://www.youtube-nocookie.com/embed/dQw4w9WgXcQ")]
    [InlineData("https://www.youtube.com/shorts/dQw4w9WgXcQ")]
    public void RecognizedFormats_ExtractTheVideoId(string input)
    {
        Assert.True(YouTubeUrlParser.TryExtractVideoId(input, out var id));
        Assert.Equal("dQw4w9WgXcQ", id);
    }

    [Theory]
    [InlineData("not a url at all")]
    [InlineData("https://vimeo.com/12345")]
    [InlineData("https://www.youtube.com/watch?v=short")]
    [InlineData("javascript:alert(1)")]
    public void UnrecognizedInput_FailsToParse(string input)
    {
        Assert.False(YouTubeUrlParser.TryExtractVideoId(input, out var id));
        Assert.Null(id);
    }
}
