using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class ProgressConfiguration : IEntityTypeConfiguration<Progress>
{
    public void Configure(EntityTypeBuilder<Progress> builder)
    {
        builder.ToTable("ProgressRecords");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>();

        builder.Property(p => p.Percentage)
            .HasPrecision(5, 2);

        // Unique constraint
        builder.HasIndex(p => new { p.StudentId, p.CourseId })
            .IsUnique()
            .HasDatabaseName("IX_Progress_StudentCourse");

        // Index for performance
        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Progress_Status");

        // Relationships
        builder.HasOne(p => p.Course)
            .WithMany(c => c.ProgressRecords)
            .HasForeignKey(p => p.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Student)
            .WithMany(s => s.ProgressRecords)
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

