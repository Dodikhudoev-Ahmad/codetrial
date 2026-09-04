using CodeTrail.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeTrail.Infrastructure.Persistence;

public class CodeTrailDbContext : DbContext
{
    public CodeTrailDbContext(DbContextOptions<CodeTrailDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
    public DbSet<ShortAnswerKey> ShortAnswerKeys => Set<ShortAnswerKey>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<LessonAttempt> LessonAttempts => Set<LessonAttempt>();
    public DbSet<AnswerSubmission> AnswerSubmissions => Set<AnswerSubmission>();
    public DbSet<VideoProgress> VideoProgress => Set<VideoProgress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CodeTrailDbContext).Assembly);
    }
}
