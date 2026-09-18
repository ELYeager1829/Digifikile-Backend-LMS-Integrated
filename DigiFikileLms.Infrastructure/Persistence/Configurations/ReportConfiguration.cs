using DigiFikileLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("Reports");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.SetaAdministrator).WithMany(x => x.Reports).HasForeignKey(x => x.SetaAdministratorId);
        builder.HasOne(x => x.TrainingProvider).WithMany(x => x.Reports).HasForeignKey(x => x.TrainingProviderId);
    }
}
