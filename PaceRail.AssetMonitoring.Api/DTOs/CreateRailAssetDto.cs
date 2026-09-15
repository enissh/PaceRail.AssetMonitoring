using System.ComponentModel.DataAnnotations;

namespace PaceRail.AssetMonitoring.Api.DTOs;

public class CreateRailAssetDto
{
    [Required(ErrorMessage = "Asset Tag is required.")]
    [StringLength(20, ErrorMessage = "Asset Tag cannot exceed 20 characters.")]
    public string AssetTag { get; set; } = string.Empty;

    [Required(ErrorMessage = "ELR is required.")]
    [StringLength(10, ErrorMessage = "ELR cannot exceed 10 characters.")]
    public string ELR { get; set; } = string.Empty;

    [Required(ErrorMessage = "Asset Name is required.")]
    public string AssetName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Asset Type is required.")]
    public string AssetType { get; set; } = string.Empty;

    public string Status { get; set; } = "Operational"; // Default to Operational if omitted

    public double StartChainageMiles { get; set; }
    public double EndChainageMiles { get; set; }
}