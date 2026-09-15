using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaceRail.AssetMonitoring.Api.Data;
using PaceRail.AssetMonitoring.Api.DTOs;
using PaceRail.AssetMonitoring.Api.Models;
using PaceRail.AssetMonitoring.Api.Services;

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
    public async Task<ActionResult<IEnumerable<RailAsset>>> GetAssets(
        [FromQuery] string? elr,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.RailAssets.AsQueryable();

        if (!string.IsNullOrWhiteSpace(elr))
            query = query.Where(a => a.ELR == elr);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(a => a.Status == status);

        var assets = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(assets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RailAsset>> GetAsset(int id)
    {
        var asset = await _context.RailAssets.FindAsync(id);
        if (asset == null) return NotFound();
        return Ok(asset);
    }

    [HttpPost]
    public async Task<ActionResult<RailAsset>> CreateAsset(CreateRailAssetDto dto)
    {
        var asset = new RailAsset
        {
            AssetTag = dto.AssetTag,
            ELR = dto.ELR,
            AssetName = dto.AssetName,
            AssetType = dto.AssetType,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Operational" : dto.Status,
            StartChainageMiles = dto.StartChainageMiles,
            EndChainageMiles = dto.EndChainageMiles,
            CreatedAt = DateTime.UtcNow
        };

        _context.RailAssets.Add(asset);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, asset);
    }

    [HttpPost("{id}/evaluate-safety")]
    public async Task<IActionResult> EvaluateSafety(
        int id, 
        [FromServices] RailChainageEvaluator evaluator)
    {
        var violations = await evaluator.EvaluateSafetyRadiusAsync(id);
        return Ok(new 
        { 
            EvaluatedAssetId = id, 
            ViolationsTriggered = violations.Count, 
            Details = violations 
        });
    }
}