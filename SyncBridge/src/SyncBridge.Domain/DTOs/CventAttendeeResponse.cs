namespace SyncBridge.Domain.DTOs;

public class AdmissionItem
{
    public string id { get; set; }
    public string code { get; set; }
    public string name { get; set; }
}

public class Contact
{
    public string createdBy { get; set; }
    public DateTime lastModified { get; set; }
    public string lastModifiedBy { get; set; }
    public string id { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string email { get; set; }
    public OptOut optOut { get; set; }
    public string sourceId { get; set; }
    public bool deleted { get; set; }
    public Type type { get; set; }
}

public class CVENTAttendeeResponse
{
    public string id { get; set; }
    public Event @event { get; set; }
    public string confirmationNumber { get; set; }
    public Contact contact { get; set; }
    public bool checkedIn { get; set; }
    public RegistrationPath registrationPath { get; set; }
    public InvitationList invitationList { get; set; }
    public AdmissionItem admissionItem { get; set; }
    public bool guest { get; set; }
    public Group group { get; set; }
    public bool unsubscribed { get; set; }
    public string status { get; set; }
    public DateTime registeredAt { get; set; }
    public WebLinks webLinks { get; set; }
    public DateTime registrationLastModified { get; set; }
    public string invitedBy { get; set; }
    public string responseMethod { get; set; }
    public bool allowAppointmentPushNotifications { get; set; }
    public bool testRecord { get; set; }
    public DateTime attendeeLastModified { get; set; }
    public bool deletedGuest { get; set; }
    public RegistrationType registrationType { get; set; }
    public string visibility { get; set; }
    public bool? showPopupNotification { get; set; }
    public bool? allowPushNotifications { get; set; }

    /// <summary>
    /// Maps this attendee DTO to a CVENT attendee entity used for persistence.
    /// </summary>
    /// <param name="source">Source system label (defaults to CVENT)</param>
    public SyncBridge.Domain.Models.CVENT.Attendee ToEntity(string source = "CVENT")
    {
        var entity = new SyncBridge.Domain.Models.CVENT.Attendee
        {
            id = id,
            // type property will default to Attendee if null; set explicitly for clarity
            type = nameof(SyncBridge.Domain.Models.CVENT.Attendee),
            EventId = @event?.id,
            FirstName = contact?.firstName,
            LastName = contact?.lastName,
            AttendeeName = string.Join(" ", new[] { contact?.firstName, contact?.lastName }.Where(s => !string.IsNullOrWhiteSpace(s))),
            RegistrationDate = registeredAt,
            Status = status,
            ExternalEventId = @event?.id, // Mirror EventId as external reference
            ExternalAttendeeId = id,
            ExternalContactId = contact?.id,
            ExternalTicketTypeId = admissionItem?.id,
            Source = source,
            // SFAttendeeId left null (no field in DTO)
        };

        // Choose created/modified semantics: use registration date as Created, most recent modification between attendee & registration mods
        var modified = attendeeLastModified > registrationLastModified ? attendeeLastModified : registrationLastModified;
        entity.CreatedDT = registeredAt;
        entity.ModifiedDT = modified;
        // CreatedBy / ModifiedBy not exposed in this DTO; leave null

        return entity;
    }
}

public class Group
{
    public bool leader { get; set; }
    public bool member { get; set; }
}

public class InvitationList
{
    public string id { get; set; }
    public string name { get; set; }
}

public class OptOut
{
    public bool optedOut { get; set; }
}

public class RegistrationPath
{
    public string id { get; set; }
    public string code { get; set; }
    public string name { get; set; }
}

public class RegistrationType
{
    public string id { get; set; }
    public string code { get; set; }
    public string name { get; set; }
}

public class Type
{
    public string id { get; set; }
    public string name { get; set; }
}

public class WebLinks
{
    public string acceptRegistration { get; set; }
    public string declineRegistration { get; set; }
}
