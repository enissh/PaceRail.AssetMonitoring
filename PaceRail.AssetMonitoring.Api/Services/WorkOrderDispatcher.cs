using PaceRail.AssetMonitoring.Api.Data;
using PaceRail.AssetMonitoring.Api.Models;

namespace PaceRail.AssetMonitoring.Api.Services;

public class WorkOrderDispatcher
{
    private readonly AppDbContext _context;

    public WorkOrderDispatcher(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WorkOrder> DispatchEmergencyWorkOrderAsync(int assetId, string reason)
    {
        var asset = await _context.RailAssets.FindAsync(assetId);
        var assetTag = asset?.AssetTag ?? $"ID:{assetId}";

        var workOrder = new WorkOrder
        {
            RailAssetId = assetId,
            Title = $"EMERGENCY INSPECTION: {assetTag}",
            Priority = "Urgent",
            Status = "Open",
            AssignedTeam = "Civils Emergency Response Unit (CERU)",
            Description = $"Automated dispatch triggered by spatial rule engine. Reason: {reason}",
            CreatedAt = DateTime.UtcNow
        };

        _context.WorkOrders.Add(workOrder);
        await _context.SaveChangesAsync();

        return workOrder;
    }
}