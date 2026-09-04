namespace CodeTrail.Application.Lessons;

// Business rule: when a lesson has a video, an attempt can't be submitted for grading
// until the student has watched at least this much of it - watching isn't optional
// decoration, it's a prerequisite for the check-yourself questions below it.
public static class VideoProgressRules
{
    public const int RequiredWatchPercent = 60;
}
