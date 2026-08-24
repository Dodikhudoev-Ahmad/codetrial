using CodeTrail.Application.Courses;
using CodeTrail.Application.Courses.Dtos;

namespace CodeTrail.Tests.Courses;

public class LessonUnlockCalculatorTests
{
    private static readonly Guid Lesson1 = Guid.NewGuid();
    private static readonly Guid Lesson2 = Guid.NewGuid();
    private static readonly Guid Lesson3 = Guid.NewGuid();

    private static readonly (Guid Id, int Order)[] ThreeLessons =
    [
        (Lesson1, 1),
        (Lesson2, 2),
        (Lesson3, 3)
    ];

    [Fact]
    public void NotEnrolled_EveryLessonIsLocked()
    {
        var statuses = LessonUnlockCalculator.ComputeStatuses(ThreeLessons, isEnrolled: false, passedLessonIds: []);

        Assert.All(statuses.Values, status => Assert.Equal(LessonStatus.Locked, status));
    }

    [Fact]
    public void EnrolledWithNoProgress_OnlyFirstLessonIsAvailable()
    {
        var statuses = LessonUnlockCalculator.ComputeStatuses(ThreeLessons, isEnrolled: true, passedLessonIds: []);

        Assert.Equal(LessonStatus.Available, statuses[Lesson1]);
        Assert.Equal(LessonStatus.Locked, statuses[Lesson2]);
        Assert.Equal(LessonStatus.Locked, statuses[Lesson3]);
    }

    [Fact]
    public void FirstLessonPassed_SecondLessonBecomesAvailable_ThirdStaysLocked()
    {
        var statuses = LessonUnlockCalculator.ComputeStatuses(ThreeLessons, isEnrolled: true, passedLessonIds: [Lesson1]);

        Assert.Equal(LessonStatus.Passed, statuses[Lesson1]);
        Assert.Equal(LessonStatus.Available, statuses[Lesson2]);
        Assert.Equal(LessonStatus.Locked, statuses[Lesson3]);
    }

    [Fact]
    public void AllLessonsPassed_AllReportPassed()
    {
        var statuses = LessonUnlockCalculator.ComputeStatuses(
            ThreeLessons, isEnrolled: true, passedLessonIds: [Lesson1, Lesson2, Lesson3]);

        Assert.All(statuses.Values, status => Assert.Equal(LessonStatus.Passed, status));
    }
}
