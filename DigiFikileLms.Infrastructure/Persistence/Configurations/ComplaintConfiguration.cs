using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigiFikileLms.Infrastructure.Persistence.Configurations;

/// <summary>
/// TYPE: ComplaintConfiguration
/// PURPOSE: PostgreSQL/EF Core mapping for the Complaint entity: table, keys, the string-backed
///          status enum, length limits that mirror the handler validation, and the required
///          restrict-delete relationship to the complainant.
/// LMS ROLE: Supports the Complaint area while respecting the Infrastructure layer boundary.
/// </summary>
public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
{
    public void Configure(EntityTypeBuilder<Complaint> builder)
    {
        builder.ToTable("Complaints");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ComplainantUserId)
            .IsRequired();

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(c => c.Category)
            .HasMaxLength(100);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>();

        builder.Property(c => c.ResolutionNotes)
            .HasMaxLength(4000);

        builder.HasIndex(c => new { c.ComplainantUserId, c.Status });

        builder.HasOne(c => c.Complainant)
            .WithMany()
            .HasForeignKey(c => c.ComplainantUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}