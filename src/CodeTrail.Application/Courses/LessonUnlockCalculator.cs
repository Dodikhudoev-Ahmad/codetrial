using CodeTrail.Application.Courses.Dtos;

namespace CodeTrail.Application.Courses;

// Single source of truth for the "lessons unlock sequentially" rule (business rule 1):
// lesson N is Available only once lesson N-1 has a passing attempt, and nothing is
// unlocked at all until the user is enrolled in the course (business rule 3).
// Shared by the course catalog (table of contents) and the lesson access guard, so the
// two never drift apart on what counts as unlocked.
public static class LessonUnlockCalculator
{
    public static IReadOnlyDictionary<Guid, LessonStatus> ComputeStatuses(
        IEnumerable<(Guid Id, int Order)> lessonsInCourseOrder, bool isEnrolled, ICollection<Guid> passedLessonIds)
    {
        var result = new Dictionary<Guid, LessonStatus>();
        var previousPassed = true;

        foreach (var lesson in lessonsInCourseOrder.OrderBy(l => l.Order))
        {
            var passed = passedLessonIds.Contains(lesson.Id);

            result[lesson.Id] = !isEnrolled
                ? LessonStatus.Locked
                : passed
                    ? LessonStatus.Passed
                    : previousPassed
                        ? LessonStatus.Available
                        : LessonStatus.Locked;

            previousPassed = passed;
        }

        return result;
    }
}
