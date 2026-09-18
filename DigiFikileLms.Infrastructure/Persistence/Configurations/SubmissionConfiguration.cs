using DigiFikileLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submissions");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Assessment).WithMany(x => x.Submissions).HasForeignKey(x => x.AssessmentId);
        builder.HasOne(x => x.Student).WithMany(s => s.Submissions).HasForeignKey(x => x.StudentId);
    }
}
