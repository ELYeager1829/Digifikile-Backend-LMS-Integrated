using DigiFikileLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class SETAProgrammeConfiguration : IEntityTypeConfiguration<SETAProgramme>
{
    public void Configure(EntityTypeBuilder<SETAProgramme> builder)
    {
        builder.ToTable("SETAProgrammes");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.SetaAdministrator).WithMany(x => x.SETAProgrammes).HasForeignKey(x => x.SetaAdministratorId);
    }
}
