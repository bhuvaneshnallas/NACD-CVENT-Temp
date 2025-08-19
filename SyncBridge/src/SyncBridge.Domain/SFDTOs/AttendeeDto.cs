using Microsoft.Extensions.Logging;

namespace CventSalesforceSyncApi.Domain.DTO
{
    public class AttendeeDto
    {
        public string Id { get; set; }
        public string? ExternalAttendeeId { get; set; }
        public string? ExternalEventId { get; set; }
        public string? ExternalTicketTypeId { get; set; }
        public string ExternalContactId { get; set; }
        public string AttendeeName { get; set; }
        public DateOnly? RegistrationDate { get; set; }
        public string? FirstName { get; set; }
        public string ?LastName { get; set; }
        public string Status { get; set; }
        public string SFEventId { get; set; } = "";
        public string SFTicketTypeId { get; set; } = "";
        public string? SFContactId { get; set; } = "";
        public string? SFAttendeeId { get; set; } = "";
        public string BatchId { get; set; } = Guid.NewGuid().ToString();


    }
}
