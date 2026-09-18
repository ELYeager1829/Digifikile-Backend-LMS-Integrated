using DigiFikileLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.Description).HasMaxLength(500);

        // Unidirectional many-to-many mapped explicitly to the ERD join table
        // "RolePermissions" with columns RoleId + PermissionId (composite PK).
        builder.HasMany(x => x.Permissions)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "RolePermissions",
                join => join
                    .HasOne<Permission>()
                    .WithMany()
                    .HasForeignKey("PermissionId")
                    .OnDelete(DeleteBehavior.Cascade),
                join => join
                    .HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("RolePermissions");
                    join.HasKey("RoleId", "PermissionId");
                });
    }
}