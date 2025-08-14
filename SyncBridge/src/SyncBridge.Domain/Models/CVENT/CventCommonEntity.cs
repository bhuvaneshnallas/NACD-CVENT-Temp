namespace SyncBridge.Domain.Models.CVENT;

public class CventCommonEntity
{
    public DateTime? CreatedDT { get; set; } = DateTime.MinValue;
    public DateTime? ModifiedDT { get; set; } = DateTime.MinValue;
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Marks the record sent for sync
    /// </summary>
    public bool SentForSync { get; set; } = false;

    /// <summary>
    /// Sets the record as new
    /// </summary>
    public bool IsNew { get; set; } = true;
}
