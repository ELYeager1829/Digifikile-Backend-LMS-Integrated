using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class GroupEnrollmentConfiguration : IEntityTypeConfiguration<GroupEnrollment>
{
    public void Configure(EntityTypeBuilder<GroupEnrollment> builder)
    {
        builder.ToTable("GroupEnrollments");

        builder.HasKey(ge => ge.Id);

        builder.Property(ge => ge.AssignedAt)
            .IsRequired();

        // Unique constraint - prevent duplicate group enrollments
        builder.HasIndex(ge => new { ge.GroupId, ge.StudentId })
            .IsUnique()
            .HasDatabaseName("IX_GroupEnrollments_GroupStudent");

        // Relationships
        builder.HasOne(ge => ge.Group)
            .WithMany(g => g.GroupEnrollments)
            .HasForeignKey(ge => ge.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ge => ge.Student)
            .WithMany()
            .HasForeignKey(ge => ge.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}