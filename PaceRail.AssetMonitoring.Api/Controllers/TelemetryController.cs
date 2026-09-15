using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaceRail.AssetMonitoring.Api.Data;

namespace PaceRail.AssetMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly AppDbContext _context;

    public TelemetryController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("asset/{assetId}")]
    public async Task<IActionResult> GetAssetTelemetry(int assetId, [FromQuery] int limit = 20)
    {
        var readings = await _context.TelemetryReadings
            .Where(t => t.RailAssetId == assetId)
            .OrderByDescending(t => t.Timestamp)
            .Take(limit)
            .ToListAsync();

        return Ok(readings);
    }
}