using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.ToTable("Certificates");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CertificateNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(c => c.CertificateNumber)
            .IsUnique()
            .HasDatabaseName("IX_Certificates_Number");

        builder.Property(c => c.CourseCode)
            .HasMaxLength(50);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.IssuedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(c => c.Result)
            .WithMany(r => r.Certificates)
            .HasForeignKey(c => c.ResultId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

