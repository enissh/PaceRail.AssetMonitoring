using PaceRail.AssetMonitoring.Api.Models;

namespace PaceRail.AssetMonitoring.Api.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        if (context.RailAssets.Any()) return; // Database already seeded

        var assets = new RailAsset[]
        {
            new RailAsset { AssetTag = "CIV-ECM1-001", ELR = "ECM1", AssetName = "Retaining Wall A1", AssetType = "Lineside Civils", Status = "Operational", CreatedAt = DateTime.UtcNow },
            new RailAsset { AssetTag = "ELE-ECM1-042", ELR = "ECM1", AssetName = "Feeder Substation B", AssetType = "Electrical Ancillary", Status = "Operational", CreatedAt = DateTime.UtcNow },
            new RailAsset { AssetTag = "LC-ECM1-109", ELR = "ECM1", AssetName = "King's Cross Barrier Gate", AssetType = "Level Crossing", Status = "Inspection Due", CreatedAt = DateTime.UtcNow }
        };

        context.RailAssets.AddRange(assets);
        context.SaveChanges();
    }
}