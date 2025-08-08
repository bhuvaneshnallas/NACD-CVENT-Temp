namespace WebApplication1.Models
{
    public class EventDto
    {
        public string id { get; set; }
        public string ExternalEventId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Category { get; set; }
        public string Capacity { get; set; }
        public string Conference { get; set; }
        public string EventFormat { get; set; }
        public DateOnly LaunchDate { get; set; }
        public string TimeZone { get; set; }
        public string? SFEventId { get; set; } = "";
        public string? BatchId { get; set; } = "";
    }
}
