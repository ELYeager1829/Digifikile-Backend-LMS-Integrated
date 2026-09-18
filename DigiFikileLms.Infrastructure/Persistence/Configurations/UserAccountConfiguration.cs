using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.ToTable("UserAccounts");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Surname).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Password).IsRequired().HasMaxLength(500);
        builder.Property(u => u.UserRole).IsRequired().HasMaxLength(50).HasConversion<string>();

        builder.HasOne(u => u.Role).WithMany().HasForeignKey(u => u.RoleId);
    }
}
