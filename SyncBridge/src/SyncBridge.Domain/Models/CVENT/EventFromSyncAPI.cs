using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Models.CVENT
{
    public class EventFromSyncAPI : BaseModel
    {
        public string? id { get; set; }
        public string? ExternalEventId { get; set; }
        public string? Title { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public string? Code { get; set; }
        public string? Category { get; set; }
        public int? Capacity { get; set; }
        public string? Conference { get; set; }
        public string? EventFormat { get; set; }
        public DateTime? LaunchDate { get; set; }
        public string? TimeZone { get; set; }
        public string? SFEventId { get; set; }

       // public string? BatchId { get; set; } = "";

    }
}
