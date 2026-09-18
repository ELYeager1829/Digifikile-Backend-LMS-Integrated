using DigiFikileLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.ToTable("Feedbacks");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Moderator).WithMany(x => x.Feedbacks).HasForeignKey(x => x.ModeratorId);
        builder.HasOne(x => x.Result).WithMany(x => x.Feedbacks).HasForeignKey(x => x.ResultId);
        builder.HasOne(x => x.Assessment).WithMany(x => x.Feedbacks).HasForeignKey(x => x.AssessmentId);
    }
}
