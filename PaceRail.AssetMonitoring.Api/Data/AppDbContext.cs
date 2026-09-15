using Microsoft.EntityFrameworkCore;
using PaceRail.AssetMonitoring.Api.Models;

namespace PaceRail.AssetMonitoring.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<RailAsset> RailAssets => Set<RailAsset>();
    public DbSet<InspectionLog> InspectionLogs => Set<InspectionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RailAsset>()
            .HasMany(a => a.Inspections)
            .WithOne()
            .HasForeignKey(i => i.RailAssetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}