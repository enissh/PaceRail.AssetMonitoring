namespace PaceRail.AssetMonitoring.Api.Models;

public class RailAsset
{
    public int Id { get; set; }
    public string AssetTag { get; set; } = string.Empty; // e.g. "CIV-ECM1-8821"
    public string ELR { get; set; } = string.Empty;      // Engineers Line Reference (e.g. "ECM1")
    public string AssetName { get; set; } = string.Empty; 
    public string AssetType { get; set; } = string.Empty; // "Lineside Civils", "Level Crossing", "Electrical Ancillary"
    public double StartChainageMiles { get; set; }
    public double EndChainageMiles { get; set; }
    public string Status { get; set; } = "Operational";  // "Operational", "Inspection Due", "Critical Defect"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<InspectionLog> Inspections { get; set; } = new();
}