using CodeTrail.Application.Attempts;
using CodeTrail.Application.Attempts.AnswerCheckers;
using CodeTrail.Domain.Entities;
using CodeTrail.Domain.Enums;
using CodeTrail.Infrastructure.Attempts;
using CodeTrail.Infrastructure.Lessons;
using CodeTrail.Infrastructure.Persistence;
using Microsoft.Extensions.Logging.Abstractions;

namespace CodeTrail.Tests.TestSupport;

// Builds a minimal, deterministic course graph - one SingleChoice question per lesson,
// with a known-correct and a known-wrong AnswerOption id - so tests can submit attempts
// without depending on seed data or real question content.
public class AttemptTestFixture
{
    public CodeTrailDbContext Db { get; }
    public Course Course { get; }
    public List<Lesson> Lessons { get; }
    public User Student { get; }
    public AttemptService AttemptService { get; }

    private AttemptTestFixture(
        CodeTrailDbContext db, Course course, List<Lesson> lessons, User student, AttemptService attemptService)
    {
        Db = db;
        Course = course;
        Lessons = lessons;
        Student = student;
        AttemptService = attemptService;
    }

    public static async Task<AttemptTestFixture> CreateAsync(int lessonCount, int xpPerLesson = 10)
    {
        var db = InMemoryDbContextFactory.Create();

        var author = new User
        {
            Email = "author@test.local",
            PasswordHash = "hash",
            DisplayName = "Author",
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        var student = new User
        {
            Email = "student@test.local",
            PasswordHash = "hash",
            DisplayName = "Student",
            Role = UserRole.Student,
            CreatedAt = DateTime.UtcNow
        };

        var course = new Course
        {
            Title = "Test Course",
            Slug = "test-course",
            Description = "Test",
            Level = CourseLevel.Beginner,
            Language = "Test",
            IsPublished = true,
            Author = author
        };

        var lessons = new List<Lesson>();

        for (var i = 1; i <= lessonCount; i++)
        {
            var question = new Question
            {
                Order = 1,
                Type = QuestionType.SingleChoice,
                Text = $"Question for lesson {i}",
                Explanation = "Because."
            };

            var correctOption = new AnswerOption { Question = question, Text = "Correct", IsCorrect = true };
            var wrongOption = new AnswerOption { Question = question, Text = "Wrong", IsCorrect = false };
            question.AnswerOptions.Add(correctOption);
            question.AnswerOptions.Add(wrongOption);

            var lesson = new Lesson
            {
                Course = course,
                Order = i,
                Title = $"Lesson {i}",
                TheoryMarkdown = "Theory",
                XpReward = xpPerLesson
            };
            question.Lesson = lesson;
            lesson.Questions.Add(question);

            course.Lessons.Add(lesson);
            lessons.Add(lesson);
        }

        var enrollment = new Enrollment
        {
            User = student,
            Course = course,
            EnrolledAt = DateTime.UtcNow
        };

        db.Users.AddRange(author, student);
        db.Courses.Add(course);
        db.Enrollments.Add(enrollment);
        await db.SaveChangesAsync();

        var accessGuard = new LessonAccessGuard(db);
        var checkerResolver = new AnswerCheckerResolver(
        [
            new SingleChoiceAnswerChecker(),
            new MultiChoiceAnswerChecker(),
            new ShortAnswerAnswerChecker()
        ]);

        var attemptService = new AttemptService(db, accessGuard, checkerResolver, NullLogger<AttemptService>.Instance);

        return new AttemptTestFixture(db, course, lessons, student, attemptService);
    }

    public Guid CorrectOptionId(int lessonOrder) =>
        Lessons[lessonOrder - 1].Questions.Single().AnswerOptions.Single(o => o.IsCorrect).Id;

    public Guid WrongOptionId(int lessonOrder) =>
        Lessons[lessonOrder - 1].Questions.Single().AnswerOptions.Single(o => !o.IsCorrect).Id;

    public Guid QuestionId(int lessonOrder) => Lessons[lessonOrder - 1].Questions.Single().Id;
}
