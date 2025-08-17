using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Models;

public class SyncQueueModel
{
    public string? id { get; set; }
    public string? module { get; set; }
    public string? companyId { get; set; }
    public string? accountId { get; set; }
    public string? dayJobAccountId { get; set; }
    public string? PartitionId { get; set; }
    public bool? IsBatchSplitRequired { get; set; }
    public int? BatchSplitRecordCount { get; set; }
}
