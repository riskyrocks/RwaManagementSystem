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
public class SubscriptionsController : ControllerBase
{
    private readonly MaintenanceDbContext _context;
    public SubscriptionsController(MaintenanceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptions()
        => await _context.Subscriptions.Include(s => s.Flat).ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Subscription>> GetSubscription(int id)
    {
        var subscription = await _context.Subscriptions.Include(s => s.Flat)
            .FirstOrDefaultAsync(s => s.SubscriptionId == id);
        if (subscription == null) return NotFound();
        return subscription;
    }

    [HttpPost]
    public async Task<ActionResult<Subscription>> CreateSubscription(Subscription subscription)
    {
        var building = await _context.Buildings.Include(b => b.Flats)
            .FirstOrDefaultAsync(b => b.BuildingId == subscription.Flat.BuildingId);

        if (building != null)
        {
            int flatsCount = building.Flats.Count;
            if (flatsCount >= 3 && flatsCount <= 5) subscription.MonthlyPrice *= 0.9m;
            else if (flatsCount >= 6 && flatsCount <= 10) subscription.MonthlyPrice *= 0.85m;
            else if (flatsCount > 10) subscription.MonthlyPrice *= 0.8m;
        }

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSubscription), new { id = subscription.SubscriptionId }, subscription);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubscription(int id, Subscription subscription)
    {
        if (id != subscription.SubscriptionId) return BadRequest();
        _context.Entry(subscription).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubscription(int id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription == null) return NotFound();
        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

}
