using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<Result>
{
    public void Configure(EntityTypeBuilder<Result> builder)
    {
        builder.ToTable("Results");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Percentage)
            .HasPrecision(5, 2);

        builder.Property(r => r.Grade)
            .HasMaxLength(10);

        // Relationships
        builder.HasOne(r => r.Student)
            .WithMany(s => s.Results)
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Certificates)
            .WithOne(c => c.Result)
            .HasForeignKey(c => c.ResultId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Feedbacks)
            .WithOne(f => f.Result)
            .HasForeignKey(f => f.ResultId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

