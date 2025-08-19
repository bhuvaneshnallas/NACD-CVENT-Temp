using System.ComponentModel.DataAnnotations;

namespace SyncBridge.Domain.Models.CVENT;

public class EventEntity : CventCommonEntity
{
    [Key]
    public string id { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public bool Virtual { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CloseAfter { get; set; }
    public DateTime ArchiveAfter { get; set; }
    public DateTime LaunchAfter { get; set; }
    public string Status { get; set; }
    public string Category { get; set; }
    public string TimeZone { get; set; }
    public string DefaultLocale { get; set; }
    public string Currency { get; set; }
    public List<string> Languages { get; set; }
    public string EventStatus { get; set; }
    public bool TestMode { get; set; }
    public string RegistrationSecurityLevel { get; set; }
    public List<Planner> Planners { get; set; }
    public DateTime? LaunchDate { get; set; } // Launch after
    public string EventFormat { get; set; } // Format
    public string Conference { get; set; } // ?
    public int? Capacity { get; set; } // ?
    public string ExternalEventId { get; set; } //?
    public string SFEventId { get; set; } //
    public string Source { get; set; } //?
}

public class Planner
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool Deleted { get; set; }
}
