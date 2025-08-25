using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Models
{
    public class SyncLogUpdate
    {
        public string? id { get; set; }
        public string? HandlerName { get; set; }
        public string? Status { get; set; }
        public string? Error { get; set; }
        public string? ErrorCode { get; set; }
        public DateTime? Timestamp { get; set; }
        public bool IsCventContact { get; set; } = false;
    }
}
