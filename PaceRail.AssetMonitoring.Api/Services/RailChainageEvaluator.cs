using Microsoft.EntityFrameworkCore;
using PaceRail.AssetMonitoring.Api.Data;
using PaceRail.AssetMonitoring.Api.Models;

namespace PaceRail.AssetMonitoring.Api.Services;

public class RailChainageEvaluator
{
    private readonly AppDbContext _context;
    private readonly WorkOrderDispatcher _dispatcher;

    public RailChainageEvaluator(AppDbContext context, WorkOrderDispatcher dispatcher)
    {
        _context = context;
        _dispatcher = dispatcher;
    }

    public async Task<List<AssetRuleViolation>> EvaluateSafetyRadiusAsync(int targetAssetId, double radiusMiles = 0.5)
    {
        var targetAsset = await _context.RailAssets.FindAsync(targetAssetId);
        if (targetAsset == null || targetAsset.Status != "Critical Defect")
            return new List<AssetRuleViolation>();

        var nearbyAssets = await _context.RailAssets
            .Where(a => a.ELR == targetAsset.ELR && a.Id != targetAsset.Id)
            .Where(a => Math.Abs(a.StartChainageMiles - targetAsset.StartChainageMiles) <= radiusMiles)
            .ToListAsync();

        var violations = new List<AssetRuleViolation>();

        foreach (var nearby in nearbyAssets)
        {
            var violation = new AssetRuleViolation
            {
                SourceAssetId = targetAsset.Id,
                AffectedAssetId = nearby.Id,
                ViolationType = "Proximity Danger Zone",
                Reason = $"Asset '{nearby.AssetTag}' is within {radiusMiles} miles of Critical Defect at '{targetAsset.AssetTag}' on line {targetAsset.ELR}.",
                FlaggedAt = DateTime.UtcNow
            };

            violations.Add(violation);

            if (nearby.Status == "Operational")
            {
                nearby.Status = "Inspection Due";
            }

            // Auto-dispatch urgent work order for each impacted asset
            await _dispatcher.DispatchEmergencyWorkOrderAsync(nearby.Id, violation.Reason);
        }

        if (violations.Any())
        {
            _context.AssetRuleViolations.AddRange(violations);
            await _context.SaveChangesAsync();
        }

        return violations;
    }
}