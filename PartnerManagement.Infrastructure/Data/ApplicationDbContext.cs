using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PartnerManagement.Core.Entities;

namespace PartnerManagement.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Partner> Partners { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventPartner> EventPartners { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply configurations from assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure EventPartner unique constraint
        builder.Entity<EventPartner>()
            .HasIndex(ep => new { ep.EventId, ep.PartnerId })
            .IsUnique();

        // Configure relationships
        builder.Entity<EventPartner>()
            .HasOne(ep => ep.Event)
            .WithMany(e => e.EventPartners)
            .HasForeignKey(ep => ep.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<EventPartner>()
            .HasOne(ep => ep.Partner)
            .WithMany(p => p.EventPartners)
            .HasForeignKey(ep => ep.PartnerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        builder.Entity<Partner>()
            .HasIndex(p => p.Name);

        builder.Entity<Partner>()
            .HasIndex(p => p.Status);

        builder.Entity<Event>()
            .HasIndex(e => e.StartDate);

        builder.Entity<Event>()
            .HasIndex(e => e.Status);
    }
}
