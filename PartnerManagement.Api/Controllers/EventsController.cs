using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartnerManagement.Core.Entities;
using PartnerManagement.Infrastructure.Data;

namespace PartnerManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EventsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null)
    {
        var query = _context.Events.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(e => e.Name.Contains(search) ||
                                    (e.Location != null && e.Location.Contains(search)));
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(e => e.Status == status);
        }

        var total = await query.CountAsync();
        var events = await query
            .OrderByDescending(e => e.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            data = events,
            page,
            pageSize,
            total,
            totalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEvent(int id)
    {
        var eventItem = await _context.Events
            .Include(e => e.EventPartners)
            .ThenInclude(ep => ep.Partner)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventItem == null)
        {
            return NotFound(new { message = "Event not found" });
        }

        return Ok(new
        {
            eventItem.Id,
            eventItem.Name,
            eventItem.Description,
            eventItem.StartDate,
            eventItem.EndDate,
            eventItem.Location,
            eventItem.Status,
            eventItem.CreatedBy,
            eventItem.CreatedAt,
            eventItem.UpdatedAt,
            Partners = eventItem.EventPartners.Select(ep => ep.Partner)
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] EventRequest request)
    {
        var eventItem = new Event
        {
            Name = request.Name,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Location = request.Location,
            Status = request.Status ?? "planned"
        };

        _context.Events.Add(eventItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEvent), new { id = eventItem.Id }, eventItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventRequest request)
    {
        var eventItem = await _context.Events.FindAsync(id);

        if (eventItem == null)
        {
            return NotFound(new { message = "Event not found" });
        }

        eventItem.Name = request.Name;
        eventItem.Description = request.Description;
        eventItem.StartDate = request.StartDate;
        eventItem.EndDate = request.EndDate;
        eventItem.Location = request.Location;
        eventItem.Status = request.Status ?? eventItem.Status;
        eventItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(eventItem);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);

        if (eventItem == null)
        {
            return NotFound(new { message = "Event not found" });
        }

        _context.Events.Remove(eventItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/partners")]
    public async Task<IActionResult> AddPartnerToEvent(int id, [FromBody] AddPartnerRequest request)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem == null)
        {
            return NotFound(new { message = "Event not found" });
        }

        var partner = await _context.Partners.FindAsync(request.PartnerId);
        if (partner == null)
        {
            return NotFound(new { message = "Partner not found" });
        }

        var existingRelation = await _context.EventPartners
            .FirstOrDefaultAsync(ep => ep.EventId == id && ep.PartnerId == request.PartnerId);

        if (existingRelation != null)
        {
            return BadRequest(new { message = "Partner already added to this event" });
        }

        var eventPartner = new EventPartner
        {
            EventId = id,
            PartnerId = request.PartnerId
        };

        _context.EventPartners.Add(eventPartner);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Partner added to event successfully" });
    }

    [HttpDelete("{id}/partners/{partnerId}")]
    public async Task<IActionResult> RemovePartnerFromEvent(int id, int partnerId)
    {
        var eventPartner = await _context.EventPartners
            .FirstOrDefaultAsync(ep => ep.EventId == id && ep.PartnerId == partnerId);

        if (eventPartner == null)
        {
            return NotFound(new { message = "Partner not associated with this event" });
        }

        _context.EventPartners.Remove(eventPartner);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/partners")]
    public async Task<IActionResult> GetEventPartners(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem == null)
        {
            return NotFound(new { message = "Event not found" });
        }

        var partners = await _context.EventPartners
            .Where(ep => ep.EventId == id)
            .Include(ep => ep.Partner)
            .Select(ep => ep.Partner)
            .ToListAsync();

        return Ok(partners);
    }
}

public record EventRequest(
    string Name,
    string? Description,
    DateTime StartDate,
    DateTime EndDate,
    string? Location,
    string? Status
);

public record AddPartnerRequest(int PartnerId);
