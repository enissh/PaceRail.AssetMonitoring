namespace PaceRail.AssetMonitoring.Api.Models;

public class WorkOrder
{
    public int Id { get; set; }
    public int RailAssetId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Priority { get; set; } = "Routine"; // Urgent, Routine, Low
    public string Status { get; set; } = "Open"; // Open, In Progress, Closed
    public string AssignedTeam { get; set; } = "Unassigned";
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
}