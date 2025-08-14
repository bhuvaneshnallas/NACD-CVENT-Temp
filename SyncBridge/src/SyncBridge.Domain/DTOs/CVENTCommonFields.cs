namespace SyncBridge.Domain.DTOs;

public class CVENTCommonFields
{
    // Audit fields
    public DateTime Created { get; set; } = DateTime.MinValue;
    public DateTime LastModified { get; set; } = DateTime.MinValue;
    public string CreatedBy { get; set; } = string.Empty;
    public string LastModifiedBy { get; set; } = string.Empty;

    /// <summary>
    /// Marks the record sent for sync
    /// </summary>
    public bool IsProcessed { get; set; } = false;
}
