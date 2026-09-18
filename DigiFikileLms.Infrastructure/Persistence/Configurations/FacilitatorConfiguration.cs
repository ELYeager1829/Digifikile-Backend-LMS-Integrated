using DigiFikileLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class FacilitatorConfiguration : IEntityTypeConfiguration<Facilitator>
{
    public void Configure(EntityTypeBuilder<Facilitator> builder)
    {
        builder.ToTable("Facilitators");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.User)
            .WithOne(u => u.Facilitator)
            .HasForeignKey<Facilitator>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Courses).WithOne(x => x.Facilitator).HasForeignKey(x => x.FacilitatorId);
    }
}
