using CodeTrail.Application.Attempts;
using CodeTrail.Application.Attempts.Dtos;
using CodeTrail.Application.Attempts.Exceptions;
using CodeTrail.Application.Lessons;
using CodeTrail.Application.Lessons.Exceptions;
using CodeTrail.Domain.Entities;
using CodeTrail.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeTrail.Infrastructure.Attempts;

public class AttemptService(
    CodeTrailDbContext db,
    ILessonAccessGuard accessGuard,
    IAnswerCheckerResolver checkerResolver) : IAttemptService
{
    private const int PassingScorePercent = 70;
    private const int MaxAttemptsPerDay = 5;

    public async Task<AttemptResultDto> SubmitAttemptAsync(Guid lessonId, Guid userId, SubmitAttemptRequest request)
    {
        var lesson = await db.Lessons
            .Include(l => l.Questions).ThenInclude(q => q.AnswerOptions)
            .Include(l => l.Questions).ThenInclude(q => q.ShortAnswerKey)
            .FirstOrDefaultAsync(l => l.Id == lessonId)
            ?? throw new LessonNotFoundException(lessonId);

        await accessGuard.EnsureUnlockedAsync(lessonId, lesson.CourseId, userId);

        var todayUtc = DateTime.UtcNow.Date;
        var attemptsToday = await db.LessonAttempts.CountAsync(a =>
            a.UserId == userId && a.LessonId == lessonId &&
            a.StartedAt >= todayUtc && a.StartedAt < todayUtc.AddDays(1));

        if (attemptsToday >= MaxAttemptsPerDay)
        {
            throw new DailyAttemptLimitExceededException(lessonId);
        }

        ValidateSubmissionCoversAllQuestions(lesson, request);

        // Must be read before this attempt is recorded, to know whether it is the
        // one that first passes the lesson (business rule 4: XP only on first pass).
        var hasPriorPass = await db.LessonAttempts
            .AnyAsync(a => a.UserId == userId && a.LessonId == lessonId && a.IsPassed);

        var previousAttemptCount = await db.LessonAttempts
            .CountAsync(a => a.UserId == userId && a.LessonId == lessonId);

        var (submissions, questionResults, correctCount) = CheckAnswers(lesson.Questions, request.Answers);

        var scorePercent = (int)Math.Round(correctCount * 100.0 / lesson.Questions.Count);
        var isPassed = scorePercent >= PassingScorePercent;
        var now = DateTime.UtcNow;

        var attempt = new LessonAttempt
        {
            UserId = userId,
            LessonId = lessonId,
            StartedAt = now,
            FinishedAt = now,
            ScorePercent = scorePercent,
            IsPassed = isPassed,
            AttemptNumber = previousAttemptCount + 1,
            AnswerSubmissions = submissions
        };

        var xpAwarded = 0;

        // Rule 12: attempt, progress, XP and streak all commit together.
        await using var transaction = await db.Database.BeginTransactionAsync();

        db.LessonAttempts.Add(attempt);
        await db.SaveChangesAsync();

        var user = await db.Users.FirstAsync(u => u.Id == userId);
        UpdateStreak(user, now);

        if (isPassed && !hasPriorPass)
        {
            xpAwarded = lesson.XpReward;
            user.TotalXp += xpAwarded;
            await MaybeCompleteEnrollmentAsync(lesson.CourseId, userId, now);
        }

        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return new AttemptResultDto
        {
            AttemptId = attempt.Id,
            ScorePercent = scorePercent,
            IsPassed = isPassed,
            AttemptNumber = attempt.AttemptNumber,
            XpAwarded = xpAwarded,
            Questions = questionResults
        };
    }

    public async Task<AttemptResultDto> GetAttemptAsync(Guid attemptId, Guid userId)
    {
        var attempt = await db.LessonAttempts
            .Include(a => a.AnswerSubmissions).ThenInclude(s => s.Question).ThenInclude(q => q.AnswerOptions)
            .Include(a => a.AnswerSubmissions).ThenInclude(s => s.Question).ThenInclude(q => q.ShortAnswerKey)
            .FirstOrDefaultAsync(a => a.Id == attemptId)
            ?? throw new AttemptNotFoundException(attemptId);

        if (attempt.UserId != userId)
        {
            throw new AttemptAccessDeniedException(attemptId);
        }

        var xpAwarded = 0;

        if (attempt.IsPassed)
        {
            var earlierPass = await db.LessonAttempts.AnyAsync(a =>
                a.UserId == attempt.UserId && a.LessonId == attempt.LessonId &&
                a.IsPassed && a.AttemptNumber < attempt.AttemptNumber);

            if (!earlierPass)
            {
                var lesson = await db.Lessons.FirstAsync(l => l.Id == attempt.LessonId);
                xpAwarded = lesson.XpReward;
            }
        }

        return new AttemptResultDto
        {
            AttemptId = attempt.Id,
            ScorePercent = attempt.ScorePercent,
            IsPassed = attempt.IsPassed,
            AttemptNumber = attempt.AttemptNumber,
            XpAwarded = xpAwarded,
            Questions = attempt.AnswerSubmissions.Select(s => new QuestionResultDto
            {
                QuestionId = s.QuestionId,
                GivenAnswer = s.GivenAnswer,
                IsCorrect = s.IsCorrect,
                Explanation = s.Question.Explanation,
                CorrectOptionIds = s.Question.AnswerOptions.Where(o => o.IsCorrect).Select(o => o.Id).ToList(),
                CorrectShortAnswer = s.Question.ShortAnswerKey?.ExpectedAnswer
            }).ToList()
        };
    }

    private static void ValidateSubmissionCoversAllQuestions(Lesson lesson, SubmitAttemptRequest request)
    {
        var questionIds = lesson.Questions.Select(q => q.Id).ToHashSet();
        var answeredIds = request.Answers.Select(a => a.QuestionId).ToList();

        if (lesson.Questions.Count == 0
            || answeredIds.Count != answeredIds.Distinct().Count()
            || !questionIds.SetEquals(answeredIds))
        {
            throw new InvalidAttemptSubmissionException(lesson.Id);
        }
    }

    private (List<AnswerSubmission> Submissions, List<QuestionResultDto> Results, int CorrectCount) CheckAnswers(
        IEnumerable<Question> questions, List<AnswerRequest> answers)
    {
        var questionById = questions.ToDictionary(q => q.Id);
        var submissions = new List<AnswerSubmission>();
        var results = new List<QuestionResultDto>();
        var correctCount = 0;

        foreach (var answer in answers)
        {
            var question = questionById[answer.QuestionId];
            var givenAnswer = answer.GivenAnswer ?? string.Empty;
            var isCorrect = checkerResolver.Resolve(question.Type).Check(question, givenAnswer);

            if (isCorrect)
            {
                correctCount++;
            }

            submissions.Add(new AnswerSubmission
            {
                QuestionId = question.Id,
                GivenAnswer = givenAnswer,
                IsCorrect = isCorrect
            });

            results.Add(new QuestionResultDto
            {
                QuestionId = question.Id,
                GivenAnswer = givenAnswer,
                IsCorrect = isCorrect,
                Explanation = question.Explanation,
                CorrectOptionIds = question.AnswerOptions.Where(o => o.IsCorrect).Select(o => o.Id).ToList(),
                CorrectShortAnswer = question.ShortAnswerKey?.ExpectedAnswer
            });
        }

        return (submissions, results, correctCount);
    }

    // Business rule 8: +1 if the previous activity was yesterday, reset to 1 if a day
    // was skipped (or this is the very first activity), unchanged if already active today.
    private static void UpdateStreak(User user, DateTime nowUtc)
    {
        var today = DateOnly.FromDateTime(nowUtc);

        if (user.LastActivityDate == today)
        {
            return;
        }

        user.CurrentStreak = user.LastActivityDate == today.AddDays(-1)
            ? user.CurrentStreak + 1
            : 1;

        user.LastActivityDate = today;
    }

    // Business rule 6: a course is complete once every one of its lessons has been passed.
    private async Task MaybeCompleteEnrollmentAsync(Guid courseId, Guid userId, DateTime now)
    {
        var totalLessons = await db.Lessons.CountAsync(l => l.CourseId == courseId);

        var passedLessons = await db.LessonAttempts
            .Where(a => a.UserId == userId && a.Lesson.CourseId == courseId && a.IsPassed)
            .Select(a => a.LessonId)
            .Distinct()
            .CountAsync();

        if (passedLessons < totalLessons)
        {
            return;
        }

        var enrollment = await db.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (enrollment is not null && enrollment.CompletedAt is null)
        {
            enrollment.CompletedAt = now;
        }
    }
}
