using CodeTrail.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeTrail.Infrastructure.Persistence.Configurations;

public class AnswerSubmissionConfiguration : IEntityTypeConfiguration<AnswerSubmission>
{
    public void Configure(EntityTypeBuilder<AnswerSubmission> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.GivenAnswer)
            .IsRequired()
            .HasMaxLength(2000);

        // Submissions are owned content of an attempt: removing the attempt removes its submissions.
        builder.HasOne(s => s.Attempt)
            .WithMany(a => a.AnswerSubmissions)
            .HasForeignKey(s => s.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        // Same rationale as LessonAttempt -> Lesson: a question with recorded submissions
        // must not be deletable via a cascading FK.
        builder.HasOne(s => s.Question)
            .WithMany(q => q.AnswerSubmissions)
            .HasForeignKey(s => s.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
