namespace SyncBridge.Domain.DTOs;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Address
{
    public string region { get; set; }
    public string regionCode { get; set; }
    public string country { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    public string address1 { get; set; }
    public string city { get; set; }
    public string countryCode { get; set; }
    public string postalCode { get; set; }
}

public class Agenda
{
    public string href { get; set; }
}

public class Category
{
    public string name { get; set; }
}

public class CventEventResponse
{
    public string id { get; set; }
    public string title { get; set; }
    public string code { get; set; }
    public bool @virtual { get; set; }
    public string format { get; set; }
    public DateTime start { get; set; }
    public DateTime end { get; set; }
    public DateTime closeAfter { get; set; }
    public DateTime archiveAfter { get; set; }
    public DateTime launchAfter { get; set; }
    public string timezone { get; set; }
    public string defaultLocale { get; set; }
    public List<string> languages { get; set; }
    public string currency { get; set; }
    public string registrationSecurityLevel { get; set; }
    public string status { get; set; }
    public string eventStatus { get; set; }
    public bool testMode { get; set; }
    public List<Planner> planners { get; set; }
    public List<object> customFields { get; set; }
    public Category category { get; set; }
    public string type { get; set; }
    public Links _links { get; set; }
    public DateTime created { get; set; }
    public DateTime lastModified { get; set; }
    public string createdBy { get; set; }
    public string lastModifiedBy { get; set; }
    public List<Venue> venues { get; set; }
    public string phone { get; set; }
    public string description { get; set; }

    public SyncBridge.Domain.Models.CVENT.EventEntity ToEntity(string source = "CVENT")
    {
        // Map planners (DTO -> Entity)
        var mappedPlanners = planners
            ?.Select(p => new SyncBridge.Domain.Models.CVENT.Planner
            {
                FirstName = p.firstName,
                LastName = p.lastName,
                Email = p.email,
                Deleted = p.deleted,
            })
            .ToList();

        var entity = new SyncBridge.Domain.Models.CVENT.EventEntity
        {
            id = id,
            Type = type,
            Title = title,
            Code = code,
            Description = description,
            Virtual = @virtual,
            StartDate = start,
            EndDate = end,
            CloseAfter = closeAfter,
            ArchiveAfter = archiveAfter,
            LaunchAfter = launchAfter,
            Status = status,
            Category = category?.name,
            TimeZone = timezone,
            DefaultLocale = defaultLocale,
            Currency = currency,
            Languages = languages ?? new List<string>(),
            EventStatus = eventStatus,
            TestMode = testMode,
            RegistrationSecurityLevel = registrationSecurityLevel,
            Planners = mappedPlanners ?? new List<SyncBridge.Domain.Models.CVENT.Planner>(),
            LaunchDate = launchAfter, // Mirrors LaunchAfter per comment in entity
            EventFormat = format, // format -> EventFormat
            // Conference (no source field) left null
            // Capacity (no source field) left at default 0
            // ExternalEventId (no source field) left null
            // SFEventId (no source field) left null
            Source = source,
        };

        // Audit fields (assuming these properties exist on the base class)
        entity.CreatedDT = created;
        entity.ModifiedDT = lastModified;
        entity.CreatedBy = createdBy;
        entity.ModifiedBy = lastModifiedBy;

        return entity;
    }
}

public class Invitation
{
    public string href { get; set; }
}

public class Planner
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string email { get; set; }
    public bool deleted { get; set; }
}

public class Registration
{
    public string href { get; set; }
}

public class Summary
{
    public string href { get; set; }
}

public class Venue
{
    public string name { get; set; }
    public Address address { get; set; }
}
