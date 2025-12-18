namespace PartnerManagement.Core.Entities;

public class EventPartner
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int PartnerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Event Event { get; set; } = null!;
    public Partner Partner { get; set; } = null!;
}
