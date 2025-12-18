using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartnerManagement.Core.Entities;
using PartnerManagement.Infrastructure.Data;

namespace PartnerManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PartnersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PartnersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetPartners(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null)
    {
        var query = _context.Partners.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Name.Contains(search) ||
                                    (p.ContactPerson != null && p.ContactPerson.Contains(search)));
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(p => p.Status == status);
        }

        var total = await query.CountAsync();
        var partners = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            data = partners,
            page,
            pageSize,
            total,
            totalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPartner(int id)
    {
        var partner = await _context.Partners.FindAsync(id);

        if (partner == null)
        {
            return NotFound(new { message = "Partner not found" });
        }

        return Ok(partner);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePartner([FromBody] PartnerRequest request)
    {
        var partner = new Partner
        {
            Name = request.Name,
            ContactPerson = request.ContactPerson,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            Description = request.Description,
            Status = request.Status ?? "active"
        };

        _context.Partners.Add(partner);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPartner), new { id = partner.Id }, partner);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePartner(int id, [FromBody] PartnerRequest request)
    {
        var partner = await _context.Partners.FindAsync(id);

        if (partner == null)
        {
            return NotFound(new { message = "Partner not found" });
        }

        partner.Name = request.Name;
        partner.ContactPerson = request.ContactPerson;
        partner.Phone = request.Phone;
        partner.Email = request.Email;
        partner.Address = request.Address;
        partner.Description = request.Description;
        partner.Status = request.Status ?? partner.Status;
        partner.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(partner);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePartner(int id)
    {
        var partner = await _context.Partners.FindAsync(id);

        if (partner == null)
        {
            return NotFound(new { message = "Partner not found" });
        }

        _context.Partners.Remove(partner);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public record PartnerRequest(
    string Name,
    string? ContactPerson,
    string? Phone,
    string? Email,
    string? Address,
    string? Description,
    string? Status
);
