using DigiFikileLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class TrainingProviderConfiguration : IEntityTypeConfiguration<TrainingProvider>
{
    public void Configure(EntityTypeBuilder<TrainingProvider> builder)
    {
        builder.ToTable("TrainingProviders");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.User)
            .WithOne(u => u.TrainingProvider)
            .HasForeignKey<TrainingProvider>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Departments).WithOne(x => x.TrainingProvider).HasForeignKey(x => x.TrainingProviderId);
        builder.HasMany(x => x.Reports).WithOne(x => x.TrainingProvider).HasForeignKey(x => x.TrainingProviderId);
    }
}
