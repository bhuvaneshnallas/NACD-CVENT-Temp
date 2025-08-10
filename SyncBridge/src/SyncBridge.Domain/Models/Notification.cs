using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Models
{
    public class Notification
    {
        public string? syncLogId { get; set; }
        public string? Application { get; set; }
        public string? Source { get; set; }
        public string? Destination { get; set; }
        public string? RecordName { get; set; }
        public string? RecordId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? Endtime { get; set; }
        public string? Status { get; set; }
        public string? Error { get; set; }
    }
}
