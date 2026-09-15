using Microsoft.EntityFrameworkCore;
using PaceRail.AssetMonitoring.Api.Models;

namespace PaceRail.AssetMonitoring.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<RailAsset> RailAssets => Set<RailAsset>();
    public DbSet<InspectionLog> InspectionLogs => Set<InspectionLog>();
    public DbSet<AssetRuleViolation> AssetRuleViolations => Set<AssetRuleViolation>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<TelemetryReading> TelemetryReadings => Set<TelemetryReading>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RailAsset>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.HasMany(a => a.Inspections)
                  .WithOne()
                  .HasForeignKey(i => i.RailAssetId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InspectionLog>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.HasIndex(i => i.RailAssetId);
        });

        modelBuilder.Entity<AssetRuleViolation>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.HasIndex(v => v.SourceAssetId);
        });

        modelBuilder.Entity<WorkOrder>(entity =>
        {
            entity.HasKey(w => w.Id);
            entity.HasIndex(w => w.RailAssetId);
            entity.HasIndex(w => w.Status);
        });

        modelBuilder.Entity<TelemetryReading>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasIndex(t => new { t.RailAssetId, t.Timestamp });
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.HasIndex(a => a.Timestamp);
        });
    }
}