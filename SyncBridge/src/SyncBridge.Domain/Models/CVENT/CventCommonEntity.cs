using System.ComponentModel.DataAnnotations;

namespace SyncBridge.Domain.Models.CVENT;

public class CventCommonEntity
{
    [Key]
    public string? id { get; set; }
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
    //public bool IsNew { get; set; } = true;
    // If CreatedDT and ModifiedDT are same, it is considered a new record. And the difference between CreatedDT and ModifiedDT should not be more than a 20 seconds
    public bool IsNew => (ModifiedDT - CreatedDT)?.TotalSeconds < 20;

}
