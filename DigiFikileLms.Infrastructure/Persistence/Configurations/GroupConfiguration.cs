using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.GroupName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(g => g.Description)
            .HasMaxLength(1000);

        builder.Property(g => g.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(g => g.setaAdministrator)
            .WithMany()
            .HasForeignKey(g => g.setaAdministratorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.setaProgramme)
            .WithMany()
            .HasForeignKey(g => g.setaProgrammeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(g => g.GroupEnrollments)
            .WithOne(ge => ge.Group)
            .HasForeignKey(ge => ge.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(g => g.GroupName)
            .HasDatabaseName("IX_Groups_GroupName");
    }
}