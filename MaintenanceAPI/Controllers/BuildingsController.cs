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
public class BuildingsController : ControllerBase
{
    private readonly MaintenanceDbContext _context;
    public BuildingsController(MaintenanceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Building>>> GetBuildings()
        => await _context.Buildings.Include(b => b.Flats).ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Building>> GetBuilding(int id)
    {
        var building = await _context.Buildings.Include(b => b.Flats)
            .FirstOrDefaultAsync(b => b.BuildingId == id);
        if (building == null) return NotFound();
        return building;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Building>> CreateBuilding(Building building)
    {
        _context.Buildings.Add(building);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBuilding), new { id = building.BuildingId }, building);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBuilding(int id, Building building)
    {
        if (id != building.BuildingId) return BadRequest();
        _context.Entry(building).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBuilding(int id)
    {
        var building = await _context.Buildings.FindAsync(id);
        if (building == null) return NotFound();
        _context.Buildings.Remove(building);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

}
