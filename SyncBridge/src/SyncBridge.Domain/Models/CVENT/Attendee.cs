using System.ComponentModel.DataAnnotations;

namespace SyncBridge.Domain.Models.CVENT;

public class Attendee : CventCommonEntity
{
    private string _type;

    public string type
    {
        get { return _type; }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                _type = nameof(Attendee);
            }
            else
            {
                _type = value;
            }
        }
    }
    public string? EventId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AttendeeName { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public string? Status { get; set; }
    public string? ExternalEventId { get; set; }
    public string? ExternalAttendeeId { get; set; }
    public string? ExternalContactId { get; set; }
    public string? ExternalTicketTypeId { get; set; }
    public string? Source { get; set; }
    public string? SFAttendeeId { get; set; }
    public string? BatchId { get; set; }
    public string? ProcessQueueId { get; set; }

}