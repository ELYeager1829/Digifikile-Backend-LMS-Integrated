using DigiFikileLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class SetaAdministratorConfiguration : IEntityTypeConfiguration<SetaAdministrator>
{
    public void Configure(EntityTypeBuilder<SetaAdministrator> builder)
    {
        builder.ToTable("SetaAdministrators");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.User)
            .WithOne(u => u.SetaAdministrator)
            .HasForeignKey<SetaAdministrator>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasMany(x => x.SETAProgrammes).WithOne(x => x.SetaAdministrator).HasForeignKey(x => x.SetaAdministratorId);
        builder.HasMany(x => x.Reports).WithOne(x => x.SetaAdministrator).HasForeignKey(x => x.SetaAdministratorId);
    }
}