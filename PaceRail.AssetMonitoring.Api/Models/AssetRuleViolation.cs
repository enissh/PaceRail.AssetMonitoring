namespace PaceRail.AssetMonitoring.Api.Models;

public class AssetRuleViolation
{
    public int Id { get; set; }
    public int SourceAssetId { get; set; }
    public int AffectedAssetId { get; set; }
    public string ViolationType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime FlaggedAt { get; set; } = DateTime.UtcNow;
}