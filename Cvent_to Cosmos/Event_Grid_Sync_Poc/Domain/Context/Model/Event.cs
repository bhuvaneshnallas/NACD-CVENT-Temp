using Event_Grid_Sync_Poc.Domain.Context.Model;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace Event_Grid_Sync_Poc.Domain.Context.Model
{
    public class Event : CommonFields
    {
        [Key]
        public string? id { get; set; }
        public string? Title { get; set; }
        public string? Code { get; set; }
        public string? BatchId { get; set; }
        public string? ProcessQueueId { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public string? Category { get; set; }
        public int? Capacity { get; set; }
        public string? Conference { get; set; }
        public string? EventFormat { get; set; }
        public DateTime? LaunchDate { get; set; }
        public string? TimeZone { get; set; }
        public string? ExternalEventId { get; set; }
        public string? Source { get; set; }
        public string? SFEventId { get; set; }
    }
}


public static class EventCaster
{
    public static Event CreateEvent(CventEvent eventItem, Guid batchId, Guid processQueueId)
    {
        return new Event
        {
            ExternalEventId = eventItem.id,
            BatchId = batchId.ToString(),
            ProcessQueueId = processQueueId.ToString(),
            Title = eventItem.Title,
            Status = "Active",
            StartDate = eventItem.Start,
            EndDate = eventItem.End,
            Description = eventItem.Description,
            Code = eventItem.Code,
            Category = eventItem.Category?.Name,
            Capacity = eventItem.Capacity,
            Conference = "Communtiy Event",
            EventFormat = "Conference",
            LaunchDate = eventItem.LaunchAfter,
            TimeZone = eventItem.Timezone,
            CreatedDT = eventItem.CreatedDT,
            ModifiedDT = eventItem.ModifiedDT,
            CreatedBy = eventItem.CreatedBy,
            ModifiedBy = eventItem.ModifiedBy,
            Source = "Cvent",
        };
    }
}