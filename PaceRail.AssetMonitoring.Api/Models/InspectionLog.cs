namespace PaceRail.AssetMonitoring.Api.Models;

public class InspectionLog
{
    public int Id { get; set; }
    public int RailAssetId { get; set; }
    public string InspectorName { get; set; } = string.Empty;
    public string Findings { get; set; } = string.Empty;
    public bool RequiresImmediateAction { get; set; }
    public DateTime InspectedAt { get; set; } = DateTime.UtcNow;
}