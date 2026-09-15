using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaceRail.AssetMonitoring.Api.Data;
using PaceRail.AssetMonitoring.Api.Models;

namespace PaceRail.AssetMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InspectionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public InspectionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<InspectionLog>> CreateInspection(InspectionLog inspection)
    {
        var asset = await _context.RailAssets.FindAsync(inspection.RailAssetId);
        if (asset == null) return NotFound("Rail asset not found.");

        if (inspection.RequiresImmediateAction)
        {
            asset.Status = "Critical Defect";
        }

        _context.InspectionLogs.Add(inspection);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(CreateInspection), new { id = inspection.Id }, inspection);
    }
}