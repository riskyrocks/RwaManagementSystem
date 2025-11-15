using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaintenanceAPI.Data;
using MaintenanceAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace MaintenanceAPI.Controllers
{
[Authorize(Roles = "Admin,Resident,Technician")]
[Route("api/[controller]")]
[ApiController]
public class ServiceRequestsController : ControllerBase
{
    private readonly MaintenanceDbContext _context;
    public ServiceRequestsController(MaintenanceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceRequest>>> GetRequests()
        => await _context.ServiceRequests.Include(s => s.Flat).Include(s => s.AssignedTechnician).ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceRequest>> GetRequest(int id)
    {
        var request = await _context.ServiceRequests.Include(s => s.Flat)
            .Include(s => s.AssignedTechnician).FirstOrDefaultAsync(s => s.ServiceRequestId == id);
        if (request == null) return NotFound();
        return request;
    }

    [HttpPost]
    [Authorize(Roles = "Resident")]
    public async Task<ActionResult<ServiceRequest>> CreateRequest(ServiceRequest request)
    {
        _context.ServiceRequests.Add(request);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRequest), new { id = request.ServiceRequestId }, request);
    }

    [HttpPut("{id}/assign/{techId}")]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> AssignTechnician(int id, int techId)
    {
        var request = await _context.ServiceRequests.FindAsync(id);
        if (request == null) return NotFound();
        request.AssignedTechId = techId;
        request.Status = "In Progress";
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var request = await _context.ServiceRequests.FindAsync(id);
        if (request == null) return NotFound();
        request.Status = status;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRequest(int id)
    {
        var request = await _context.ServiceRequests.FindAsync(id);
        if (request == null) return NotFound();
        _context.ServiceRequests.Remove(request);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

}
