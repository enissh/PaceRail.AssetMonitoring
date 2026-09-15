namespace PaceRail.AssetMonitoring.Api.Models;

public class TelemetryReading
{
    public long Id { get; set; }
    public int RailAssetId { get; set; }
    public string MetricType { get; set; } = string.Empty; // TrackTemperature, VibrationLevel, WireTension
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}