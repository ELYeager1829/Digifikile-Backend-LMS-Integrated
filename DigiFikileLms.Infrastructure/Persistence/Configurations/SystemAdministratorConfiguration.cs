using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class SystemAdministratorConfiguration : IEntityTypeConfiguration<SystemAdministrator>
{
    public void Configure(EntityTypeBuilder<SystemAdministrator> builder)
    {
        builder.ToTable("SystemAdministrators");

        builder.HasKey(sa => sa.Id);

        builder.Property(sa => sa.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Relationships
        builder.HasOne(sa => sa.User)
            .WithOne(u => u.SystemAdministrator)
            .HasForeignKey<SystemAdministrator>(sa => sa.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sa => sa.SystemLogs)
            .WithOne(sl => sl.SystemAdministrator)
            .HasForeignKey(sl => sl.SystemAdministratorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}