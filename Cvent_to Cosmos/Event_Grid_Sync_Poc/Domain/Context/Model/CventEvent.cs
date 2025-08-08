namespace Event_Grid_Sync_Poc.Domain.Context.Model
{
    public class CventEvent : CommonFields
    {
        public string? id { get; set; }
        public string? Title { get; set; }
        public string? Code { get; set; }
        public bool? Virtual { get; set; }
        public string? Format { get; set; }
        public string? Description { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public DateTime? CloseAfter { get; set; }
        public DateTime? ArchiveAfter { get; set; }
        public DateTime? LaunchAfter { get; set; }
        public string? Timezone { get; set; }
        public string? Status { get; set; }
        public List<EventVenue>? Venues { get; set; }
        public EventCategory? Category { get; set; }
        public string? Phone { get; set; }
        public string? DefaultLocale { get; set; }
        public int? Capacity { get; set; }
    }

    public class EventVenue
    {
        public string? Name { get; set; }
        public VenueAddress Address { get; set; }
        public string? Phone { get; set; }
    }

    public class VenueAddress
    {
        public string? Region { get; set; }
        public string? RegionCode { get; set; }
        public string? Country { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Address1 { get; set; }
        public string? City { get; set; }
        public string? CountryCode { get; set; }
        public string? PostalCode { get; set; }
    }

    public class EventCategory
    {
        public string? Name { get; set; }
    }
}

