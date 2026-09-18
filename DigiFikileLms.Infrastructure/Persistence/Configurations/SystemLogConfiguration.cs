using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
{
    public void Configure(EntityTypeBuilder<SystemLog> builder)
    {
        builder.ToTable("SystemLogs");

        builder.HasKey(sl => sl.Id);

        builder.Property(sl => sl.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(sl => sl.ResourceType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sl => sl.ResourceId)
            .HasMaxLength(100);

        builder.Property(sl => sl.Description)
            .HasMaxLength(2000);

        builder.Property(sl => sl.IpAddress)
            .HasMaxLength(45);

        builder.Property(sl => sl.UserAgent)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(sl => sl.SystemAdministrator)
            .WithMany(sa => sa.SystemLogs)
            .HasForeignKey(sl => sl.SystemAdministratorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(sl => sl.Action)
            .HasDatabaseName("IX_SystemLogs_Action");

        builder.HasIndex(sl => sl.ResourceType)
            .HasDatabaseName("IX_SystemLogs_ResourceType");

        builder.HasIndex(sl => sl.CreatedAt)
            .HasDatabaseName("IX_SystemLogs_CreatedAt");
    }
}