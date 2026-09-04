using CodeTrail.Infrastructure.Lessons;
using CodeTrail.Tests.TestSupport;

namespace CodeTrail.Tests.Lessons;

public class LessonServiceVideoProgressTests
{
    [Fact]
    public async Task FirstReport_IsStoredAsIs()
    {
        var fixture = await AttemptTestFixture.CreateAsync(lessonCount: 1);
        var service = new LessonService(fixture.Db, new LessonAccessGuard(fixture.Db));

        var result = await service.UpdateVideoProgressAsync(fixture.Lessons[0].Id, fixture.Student.Id, 35);

        Assert.Equal(35, result.WatchedPercent);
    }

    [Fact]
    public async Task LaterReport_OnlyMovesProgressForward()
    {
        var fixture = await AttemptTestFixture.CreateAsync(lessonCount: 1);
        var service = new LessonService(fixture.Db, new LessonAccessGuard(fixture.Db));
        var lessonId = fixture.Lessons[0].Id;

        await service.UpdateVideoProgressAsync(lessonId, fixture.Student.Id, 80);
        var afterSeekingBack = await service.UpdateVideoProgressAsync(lessonId, fixture.Student.Id, 20);

        Assert.Equal(80, afterSeekingBack.WatchedPercent);
    }

    [Theory]
    [InlineData(-10, 0)]
    [InlineData(150, 100)]
    public async Task OutOfRangeReport_IsClamped(int reported, int expected)
    {
        var fixture = await AttemptTestFixture.CreateAsync(lessonCount: 1);
        var service = new LessonService(fixture.Db, new LessonAccessGuard(fixture.Db));

        var result = await service.UpdateVideoProgressAsync(fixture.Lessons[0].Id, fixture.Student.Id, reported);

        Assert.Equal(expected, result.WatchedPercent);
    }
}
