namespace PaceRail.AssetMonitoring.Api.Models;

public class AuditLog
{
    public long Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Create, Update, Delete
    public string EntityId { get; set; } = string.Empty;
    public string Changes { get; set; } = string.Empty; // JSON details
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}