using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaintenanceAPI.Data;
using MaintenanceAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace MaintenanceAPI.Controllers
{
[Authorize(Roles = "Admin,Resident")]
[Route("api/[controller]")]
[ApiController]
public class FlatsController : ControllerBase
{
    private readonly MaintenanceDbContext _context;
    public FlatsController(MaintenanceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Flat>>> GetFlats()
        => await _context.Flats.Include(f => f.Owner).Include(f => f.Building).ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Flat>> GetFlat(int id)
    {
        var flat = await _context.Flats.Include(f => f.Owner).Include(f => f.Building)
            .FirstOrDefaultAsync(f => f.FlatId == id);
        if (flat == null) return NotFound();
        return flat;
    }

    [HttpGet("building/{buildingId}")]
    public async Task<ActionResult<IEnumerable<Flat>>> GetFlatsByBuilding(int buildingId)
        => await _context.Flats.Where(f => f.BuildingId == buildingId)
            .Include(f => f.Owner).ToListAsync();

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Flat>> CreateFlat(Flat flat)
    {
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetFlat), new { id = flat.FlatId }, flat);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFlat(int id, Flat flat)
    {
        if (id != flat.FlatId) return BadRequest();
        _context.Entry(flat).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFlat(int id)
    {
        var flat = await _context.Flats.FindAsync(id);
        if (flat == null) return NotFound();
        _context.Flats.Remove(flat);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

    }

