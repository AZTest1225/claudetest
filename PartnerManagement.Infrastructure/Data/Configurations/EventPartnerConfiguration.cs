using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnerManagement.Core.Entities;

namespace PartnerManagement.Infrastructure.Data.Configurations;

public class EventPartnerConfiguration : IEntityTypeConfiguration<EventPartner>
{
    public void Configure(EntityTypeBuilder<EventPartner> builder)
    {
        builder.ToTable("EventPartners");

        builder.HasKey(ep => ep.Id);

        builder.Property(ep => ep.EventId)
            .IsRequired();

        builder.Property(ep => ep.PartnerId)
            .IsRequired();

        builder.Property(ep => ep.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Configure relationships
        builder.HasOne(ep => ep.Event)
            .WithMany(e => e.EventPartners)
            .HasForeignKey(ep => ep.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ep => ep.Partner)
            .WithMany(p => p.EventPartners)
            .HasForeignKey(ep => ep.PartnerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint
        builder.HasIndex(ep => new { ep.EventId, ep.PartnerId })
            .IsUnique()
            .HasDatabaseName("IX_EventPartners_EventId_PartnerId");

        // Individual indexes
        builder.HasIndex(ep => ep.EventId)
            .HasDatabaseName("IX_EventPartners_EventId");

        builder.HasIndex(ep => ep.PartnerId)
            .HasDatabaseName("IX_EventPartners_PartnerId");
    }
}
