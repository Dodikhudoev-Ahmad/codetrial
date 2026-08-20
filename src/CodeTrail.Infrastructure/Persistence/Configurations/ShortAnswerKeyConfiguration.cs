using CodeTrail.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeTrail.Infrastructure.Persistence.Configurations;

public class ShortAnswerKeyConfiguration : IEntityTypeConfiguration<ShortAnswerKey>
{
    public void Configure(EntityTypeBuilder<ShortAnswerKey> builder)
    {
        // One-to-one with Question, sharing the same primary key.
        builder.HasKey(s => s.QuestionId);

        builder.Property(s => s.ExpectedAnswer)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(s => s.Question)
            .WithOne(q => q.ShortAnswerKey)
            .HasForeignKey<ShortAnswerKey>(s => s.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
