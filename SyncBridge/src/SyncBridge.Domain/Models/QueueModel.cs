using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Models
{
    public class QueueModel
    {
        public string? id { get; set; }
        public string? module { get; set; }
        public string? recordId { get; set; }
        public string? companyId { get; set; }
        public string? accountId { get; set; }
        public string? dayJobAccountId { get; set; }
        public string? PartitionId { get; set; }
        public bool? IsBatchSplitRequired { get; set; }
        public int? BatchSplitRecordCount { get; set; }
        public DateTime? moduleCreatedAt { get; set; }
        public DateTime? moduleUpdatedAt { get; set; }
        public string? moduleCreatedBy { get; set; }
        public string? moduleUpdatedBy { get; set; }
        public string? action { get; set; }
    }
}
