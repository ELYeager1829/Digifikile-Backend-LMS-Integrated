using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class AssessmentConfiguration : IEntityTypeConfiguration<Assessment>
{
    public void Configure(EntityTypeBuilder<Assessment> builder)
    {
        builder.ToTable("Assessments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.AssessmentType)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>();

        builder.Property(a => a.Grades)
            .HasPrecision(10, 2);

        // Indexes
        builder.HasIndex(a => a.AssessmentType)
            .HasDatabaseName("IX_Assessments_Type");

        // Relationships
        builder.HasOne(a => a.Module)
            .WithMany(m => m.Assessments)
            .HasForeignKey(a => a.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Student)
            .WithMany(s => s.Assessments)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Submissions)
            .WithOne(s => s.Assessment)
            .HasForeignKey(s => s.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

