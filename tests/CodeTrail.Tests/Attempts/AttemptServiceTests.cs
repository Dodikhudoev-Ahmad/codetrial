using CodeTrail.Application.Attempts.Dtos;
using CodeTrail.Application.Attempts.Exceptions;
using CodeTrail.Application.Lessons;
using CodeTrail.Application.Lessons.Exceptions;
using CodeTrail.Domain.Entities;
using CodeTrail.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;

namespace CodeTrail.Tests.Attempts;

public class AttemptServiceTests
{
    private static SubmitAttemptRequest Answer(Guid questionId, Guid optionId) => new()
    {
        Answers = [new AnswerRequest { QuestionId = questionId, GivenAnswer = optionId.ToString() }]
    };

    [Fact]
    public async Task PassingFirstLesson_UnlocksTheSecondLesson()
    {
        var fixture = await AttemptTestFixture.CreateAsync(lessonCount: 2);
        var lesson1Id = fixture.Lessons[0].Id;
        var lesson2Id = fixture.Lessons[1].Id;

        // Before lesson 1 is passed, lesson 2 must reject a submission as locked.
        await Assert.ThrowsAsync<LessonLockedException>(() =>
            fixture.AttemptService.SubmitAttemptAsync(
                lesson2Id, fixture.Student.Id, Answer(fixture.QuestionId(2), fixture.CorrectOptionId(2))));

        var request = Answer(fixture.QuestionId(1), fixture.CorrectOptionId(1));
        var result = await fixture.AttemptService.SubmitAttemptAsync(lesson1Id, fixture.Student.Id, request);

        Assert.True(result.IsPassed);

        // Lesson 2 must now accept a submission instead of throwing LessonLockedException.
        var lesson2Result = await fixture.AttemptService.SubmitAttemptAsync(
            lesson2Id, fixture.Student.Id, Answer(fixture.QuestionId(2), fixture.CorrectOptionId(2)));

        Assert.True(lesson2Result.IsPassed);
    }

    [Fact]
    public async Task FailingAnAttempt_DoesNotUnlockTheNextLesson()
    {
        var fixture = await AttemptTestFixture.CreateAsync(lessonCount: 2);
        var lesson1Id = fixture.Lessons[0].Id;
        var lesson2Id = fixture.Lessons[1].Id;

        var request = Answer(fixture.QuestionId(1), fixture.WrongOptionId(1));
        var result = await fixture.AttemptService.SubmitAttemptAsync(lesson1Id, fixture.Student.Id, request);

        Assert.False(result.IsPassed);

        await Assert.ThrowsAsync<LessonLockedException>(() =>
            fixture.AttemptService.SubmitAttemptAsync(
                lesson2Id, fixture.Student.Id, Answer(fixture.QuestionId(2), fixture.CorrectOptionId(2))));
    }

    [Fact]
    public async Task XpIsAwardedOnlyOnTheFirstSuccessfulPass()
    {
        var fixture = await AttemptTestFixture.CreateAsync(lessonCount: 1, xpPerLesson: 25);
        var lessonId = fixture.Lessons[0].Id;
        var request = Answer(fixture.QuestionId(1), fixture.CorrectOptionId(1));

        var first = await fixture.AttemptService.SubmitAttemptAsync(lessonId, fixture.Student.Id, request);
        var second = await fixture.AttemptService.SubmitAttemptAsync(lessonId, fixture.Student.Id, request);

        Assert.Equal(25, first.XpAwarded);
        Assert.Equal(0, second.XpAwarded);

        var student = await fixture.Db.Users.FirstAsync(u => u.Id == fixture.Student.Id);
        Assert.Equal(25, student.TotalXp);
    }

    [Fact]
    public async Task SixthAttemptOnTheSameLessonSameDay_IsRejected()
    {
        var fixture = await AttemptTestFixture.CreateAsync(lessonCount: 1);
        var lessonId = fixture.Lessons[0].Id;
        var request = Answer(fixture.QuestionId(1), fixture.WrongOptionId(1));

        for (var i = 0; i < 5; i++)
        {
            await fixture.AttemptService.SubmitAttemptAsync(lessonId, fixture.Student.Id, request);
        }

        await Assert.ThrowsAsync<DailyAttemptLimitExceededException>(() =>
            fixture.AttemptService.SubmitAttemptAsync(lessonId, fixture.Student.Id, request));
    }

    [Fact]
    public async Task PassingEveryLessonInACourse_MarksTheEnrollmentComplete()
    {
        var fixture = await AttemptTestFixture.CreateAsync(lessonCount: 2);

        await fixture.AttemptService.SubmitAttemptAsync(
            fixture.Lessons[0].Id, fixture.Student.Id, Answer(fixture.QuestionId(1), fixture.CorrectOptionId(1)));
        await fixture.AttemptService.SubmitAttemptAsync(
            fixture.Lessons[1].Id, fixture.Student.Id, Answer(fixture.QuestionId(2), fixture.CorrectOptionId(2)));

        var enrollment = await fixture.Db.Enrollments
            .FirstAsync(e => e.UserId == fixture.Student.Id && e.CourseId == fixture.Course.Id);

        Assert.NotNull(enrollment.CompletedAt);
    }

    [Fact]
    public async Task SubmissionMissingAQuestion_IsRejected()
    {
        var fixture = await AttemptTestFixture.CreateAsync(lessonCount: 1);
        var lessonId = fixture.Lessons[0].Id;

        await Assert.ThrowsAsync<InvalidAttemptSubmissionException>(() =>
            fixture.AttemptService.SubmitAttemptAsync(lessonId, fixture.Student.Id, new SubmitAttemptRequest { Answers = [] }));
    }

    [Fact]
    public async Task LessonWithVideo_RejectsAttemptBelowTheWatchThreshold()
    {
        var fixture = await AttemptTestFixture.CreateAsync(lessonCount: 1);
        var lessonId = fixture.Lessons[0].Id;
        fixture.Lessons[0].YouTubeVideoId = "dQw4w9WgXcQ";
        await fixture.Db.SaveChangesAsync();

        var request = Answer(fixture.QuestionId(1), fixture.CorrectOptionId(1));

        await Assert.ThrowsAsync<VideoNotWatchedException>(() =>
            fixture.AttemptService.SubmitAttemptAsync(lessonId, fixture.Student.Id, request));
    }

    [Fact]
    public async Task LessonWithVideo_AcceptsAttemptOnceWatchThresholdIsMet()
    {
        var fixture = await AttemptTestFixture.CreateAsync(lessonCount: 1);
        var lessonId = fixture.Lessons[0].Id;
        fixture.Lessons[0].YouTubeVideoId = "dQw4w9WgXcQ";
        fixture.Db.VideoProgress.Add(new VideoProgress
        {
            UserId = fixture.Student.Id,
            LessonId = lessonId,
            WatchedPercent = VideoProgressRules.RequiredWatchPercent,
            UpdatedAt = DateTime.UtcNow
        });
        await fixture.Db.SaveChangesAsync();

        var request = Answer(fixture.QuestionId(1), fixture.CorrectOptionId(1));
        var result = await fixture.AttemptService.SubmitAttemptAsync(lessonId, fixture.Student.Id, request);

        Assert.True(result.IsPassed);
    }

    [Fact]
    public async Task LessonWithoutVideo_IgnoresWatchProgressEntirely()
    {
        var fixture = await AttemptTestFixture.CreateAsync(lessonCount: 1);
        var lessonId = fixture.Lessons[0].Id;
        var request = Answer(fixture.QuestionId(1), fixture.CorrectOptionId(1));

        var result = await fixture.AttemptService.SubmitAttemptAsync(lessonId, fixture.Student.Id, request);

        Assert.True(result.IsPassed);
    }
}
