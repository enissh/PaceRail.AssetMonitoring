using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaceRail.AssetMonitoring.Api.Data;

namespace PaceRail.AssetMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkOrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public WorkOrdersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetWorkOrders([FromQuery] string? status)
    {
        var query = _context.WorkOrders.AsQueryable();
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(w => w.Status == status);
        }

        var workOrders = await query.OrderByDescending(w => w.CreatedAt).ToListAsync();
        return Ok(workOrders);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
    {
        var workOrder = await _context.WorkOrders.FindAsync(id);
        if (workOrder == null) return NotFound();

        workOrder.Status = newStatus;
        if (newStatus == "Closed")
        {
            workOrder.ResolvedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return Ok(workOrder);
    }
}