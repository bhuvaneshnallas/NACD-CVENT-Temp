using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Models;

public class SyncLog
{
    public string? id { get; set; }
    public string? Source { get; set; }
    public string? Destination { get; set; }
    public string? Module { get; set; }
    public string? RecordId { get; set; }
    public DateTime? ModuleCreatedAt { get; set; }
    public DateTime? ModuleUpdatedAt { get; set; }
    public string? ModuleCreatedBy { get; set; }
    public string? ModuleUpdatedBy { get; set; }
    public string? Action { get; set; }
    public string? Status { get; set; }
    public DateTime? SyncLastAttempt { get; set; }
    public DateTime? SyncCreatedAt { get; set; }
    public int? RetryCount { get; set; }
    public List<History>? History { get; set; }
}

public class History
{
    public string? id { get; set; }
    public string? HandlerName { get; set; }
    public string? Message { get; set; }
    public string? Status { get; set; }
    public string? Error { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime? Timestamp { get; set; }
}
