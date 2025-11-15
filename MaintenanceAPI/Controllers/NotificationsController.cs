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
    public class NotificationsController : ControllerBase
    {
        private readonly MaintenanceDbContext _context;
        public NotificationsController(MaintenanceDbContext context) => _context = context;

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetUserNotifications(int userId)
        {
            return await _context.Notifications.Where(n => n.UserId == userId).ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<Notification>> CreateNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserNotifications), new { userId = notification.UserId }, notification);
        }

        [HttpPut("{id}/read")]
        [Authorize(Roles = "Resident,Technician,Admin")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();
            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
