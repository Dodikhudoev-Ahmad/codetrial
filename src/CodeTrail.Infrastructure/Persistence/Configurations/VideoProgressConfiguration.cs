using CodeTrail.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeTrail.Infrastructure.Persistence.Configurations;

public class VideoProgressConfiguration : IEntityTypeConfiguration<VideoProgress>
{
    public void Configure(EntityTypeBuilder<VideoProgress> builder)
    {
        builder.HasKey(v => v.Id);

        builder.HasIndex(v => new { v.UserId, v.LessonId })
            .IsUnique();

        // Watch progress is disposable tracking data, not a graded record - unlike
        // LessonAttempt it's fine for it to disappear if the lesson itself is deleted.
        builder.HasOne(v => v.User)
            .WithMany()
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Lesson)
            .WithMany()
            .HasForeignKey(v => v.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
