using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<IntrusionDetection> IntrusionDetections { get; set; }
    public DbSet<Alert> Alerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IntrusionDetection>(entity =>
        {
            entity.HasIndex(i => i.DetectedAt);
            entity.HasIndex(i => i.Severity);
            entity.HasIndex(i => i.IsResolved);

            entity.HasOne(i => i.ResolvedBy)
                .WithMany()
                .HasForeignKey(i => i.ResolvedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasIndex(a => a.CreatedAt);
            entity.HasIndex(a => a.IsAcknowledged);

            entity.HasOne(a => a.AcknowledgedBy)
                .WithMany()
                .HasForeignKey(a => a.AcknowledgedById)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
}
