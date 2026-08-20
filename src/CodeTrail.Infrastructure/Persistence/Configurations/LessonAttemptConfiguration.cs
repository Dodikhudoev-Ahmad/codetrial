using CodeTrail.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeTrail.Infrastructure.Persistence.Configurations;

public class LessonAttemptConfiguration : IEntityTypeConfiguration<LessonAttempt>
{
    public void Configure(EntityTypeBuilder<LessonAttempt> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.User)
            .WithMany(u => u.LessonAttempts)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // A lesson with recorded attempts must not be deletable via a cascading FK
        // (business rule: deletion of lessons/questions with attempts is forbidden or soft).
        builder.HasOne(a => a.Lesson)
            .WithMany(l => l.Attempts)
            .HasForeignKey(a => a.LessonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
