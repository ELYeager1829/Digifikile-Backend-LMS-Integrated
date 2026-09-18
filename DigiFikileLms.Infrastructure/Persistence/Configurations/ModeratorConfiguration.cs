using DigiFikileLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class ModeratorConfiguration : IEntityTypeConfiguration<Moderator>
{
    public void Configure(EntityTypeBuilder<Moderator> builder)
    {
        builder.ToTable("Moderators");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.User)
            .WithOne(u => u.Moderator)
            .HasForeignKey<Moderator>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasMany(x => x.Feedbacks).WithOne(x => x.Moderator).HasForeignKey(x => x.ModeratorId);
    }
}