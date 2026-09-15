using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaceRail.AssetMonitoring.Api.Data;
using PaceRail.AssetMonitoring.Api.Models;

namespace PaceRail.AssetMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AssetsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RailAsset>>> GetAssets()
    {
        return await _context.RailAssets.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RailAsset>> GetAsset(int id)
    {
        var asset = await _context.RailAssets.FindAsync(id);
        if (asset == null) return NotFound();
        return asset;
    }

    [HttpPost]
    public async Task<ActionResult<RailAsset>> CreateAsset(RailAsset asset)
    {
        _context.RailAssets.Add(asset);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, asset);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsset(int id)
    {
        var asset = await _context.RailAssets.FindAsync(id);
        if (asset == null) return NotFound();

        _context.RailAssets.Remove(asset);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}