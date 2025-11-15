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
public class PaymentsController : ControllerBase
{
    private readonly MaintenanceDbContext _context;
    public PaymentsController(MaintenanceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        => await _context.Payments.Include(p => p.Subscription).ToListAsync();

    [HttpPost]
    [Authorize(Roles = "Resident")]
    public async Task<ActionResult<Payment>> CreatePayment(Payment payment)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPayments), new { id = payment.PaymentId }, payment);
    }
}

}
