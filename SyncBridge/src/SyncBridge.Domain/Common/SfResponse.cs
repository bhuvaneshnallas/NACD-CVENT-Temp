using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Common
{
    public class SfResponse
    {
        public string? Id { get; set; }
        public string? SfId { get; set; }
        public string? SfAttendeeId { get; set; }
        public string? SfReceiptId { get; set; }
        public string? BatchId { get; set; }
    }
}
