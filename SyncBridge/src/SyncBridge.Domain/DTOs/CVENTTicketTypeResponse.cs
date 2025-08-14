namespace SyncBridge.Domain.DTOs;

public class CVENTTicketTypeResponse
{
    public string id { get; set; }
    public string name { get; set; }
    public string code { get; set; }
    public DateTime lastModified { get; set; }
    public DateTime created { get; set; }
    public bool allowOptionalSessions { get; set; }
    public List<object> limitedAvailableSessions { get; set; }
    public Event @event { get; set; }
    public List<object> includedSessions { get; set; }
    public string description { get; set; }

    /// <summary>
    /// Maps this ticket type DTO to the domain TicketType entity.
    /// </summary>
    /// <param name="source">Source system label (defaults to CVENT)</param>
    public SyncBridge.Domain.Models.CVENT.TicketType ToEntity(string source = "CVENT")
    {
        var entity = new SyncBridge.Domain.Models.CVENT.TicketType
        {
            id = id,
            EventId = @event?.id,
            ProductName = name,
            ProductId = id, // Using same id unless a distinct product id provided
            // FeeId not available in DTO
            // Price not available in DTO
            ExternalTicketTypeId = id,
            ExternalEventId = @event?.id,
            Source = source,
            // SFTicketTypeId left null
        };

        entity.CreatedDT = created;
        entity.ModifiedDT = lastModified;

        return entity;
    }
}
