using CodeTrail.Application.Attempts;
using CodeTrail.Application.Attempts.Dtos;
using CodeTrail.Application.Attempts.Exceptions;
using CodeTrail.Application.Lessons;
using CodeTrail.Application.Lessons.Exceptions;
using CodeTrail.Domain.Entities;
using CodeTrail.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CodeTrail.Infrastructure.Attempts;

public class AttemptService(
    CodeTrailDbContext db,
    ILessonAccessGuard accessGuard,
    IAnswerCheckerResolver checkerResolver,
    ILogger<AttemptService> logger) : IAttemptService
{
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

        // Must be read before this attempt is recorded, to know whether it is the one
        // that first passes the lesson (business rule 4: XP only on first pass).
        var hasPriorPass = await db.LessonAttempts
            .AnyAsync(a => a.UserId == userId && a.LessonId == lessonId && a.IsPassed);

        var previousAttemptCount = await db.LessonAttempts
            .CountAsync(a => a.UserId == userId && a.LessonId == lessonId);

        var (submissions, questionResults, correctCount) = CheckAnswers(lesson.Questions, request.Answers);

        var scorePercent = AttemptScoreCalculator.CalculateScorePercent(correctCount, lesson.Questions.Count);
        var isPassed = AttemptScoreCalculator.IsPassing(scorePercent);
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

        db.LessonAttempts.Add(attempt);

        var user = await db.Users.FirstAsync(u => u.Id == userId);
        var (newStreak, newLastActivityDate) = StreakCalculator.Apply(
            user.CurrentStreak, user.LastActivityDate, DateOnly.FromDateTime(now));
        user.CurrentStreak = newStreak;
        user.LastActivityDate = newLastActivityDate;

        var xpAwarded = 0;

        if (isPassed && !hasPriorPass)
        {
            xpAwarded = lesson.XpReward;
            user.TotalXp += xpAwarded;
            await MaybeCompleteEnrollmentAsync(lesson.CourseId, userId, lessonId, now);
        }

        // Rule 12: attempt, XP, streak and course completion commit together - a single
        // SaveChanges call is already atomic for a relational provider, no explicit
        // transaction needed.
        await db.SaveChangesAsync();

        logger.LogInformation(
            "User {UserId} submitted attempt {AttemptId} for lesson {LessonId}: {ScorePercent}% ({Correct}/{Total}), passed={IsPassed}, xpAwarded={XpAwarded}",
            userId, attempt.Id, lessonId, scorePercent, correctCount, lesson.Questions.Count, isPassed, xpAwarded);

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

    // Business rule 6: a course is complete once every one of its lessons has been passed.
    // The lesson just passed by this attempt hasn't been persisted yet (SaveChanges runs
    // once, at the end), so it's folded into the passed set explicitly rather than
    // re-queried from the database.
    private async Task MaybeCompleteEnrollmentAsync(Guid courseId, Guid userId, Guid justPassedLessonId, DateTime now)
    {
        var totalLessons = await db.Lessons.CountAsync(l => l.CourseId == courseId);

        var passedLessonIds = (await db.LessonAttempts
            .Where(a => a.UserId == userId && a.Lesson.CourseId == courseId && a.IsPassed)
            .Select(a => a.LessonId)
            .Distinct()
            .ToListAsync())
            .ToHashSet();

        passedLessonIds.Add(justPassedLessonId);

        if (passedLessonIds.Count < totalLessons)
        {
            return;
        }

        var enrollment = await db.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (enrollment is not null && enrollment.CompletedAt is null)
        {
            enrollment.CompletedAt = now;
            logger.LogInformation("User {UserId} completed course {CourseId}", userId, courseId);
        }
    }
}
