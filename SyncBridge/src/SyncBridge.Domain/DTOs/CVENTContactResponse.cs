namespace SyncBridge.Domain.DTOs;

public class CVENTContactResponse
{
    public string id { get; set; }
    public DateTime created { get; set; }
    public string createdBy { get; set; }
    public DateTime lastModified { get; set; }
    public string lastModifiedBy { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string email { get; set; }
    public Type type { get; set; }
    public string primaryAddressType { get; set; }
    public bool deleted { get; set; }
    public OptOut optOut { get; set; }
    public string sourceId { get; set; }

    /// <summary>
    /// Maps this contact DTO to the domain Contact entity.
    /// </summary>
    /// <param name="source">Source system label (defaults to CVENT)</param>
    public SyncBridge.Domain.Models.CVENT.Contact ToEntity(string source = "CVENT")
    {
        var entity = new SyncBridge.Domain.Models.CVENT.Contact
        {
            id = id,
        };

        entity.CreatedDT = created;
        entity.ModifiedDT = lastModified;
        entity.CreatedBy = createdBy;
        entity.ModifiedBy = lastModifiedBy;
        // Additional name/email fields are not present on Contact entity currently; extend entity if needed.
        return entity;
    }
}
