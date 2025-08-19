using CventSalesforceSyncApi.Domain.DTO;

namespace CventSalesforceSyncApi.Domain.DTO
{
    public class SfWrapperDto
    {
        public EventDto EventDto { get; set; } = null;
        public VenueDto VenueDto { get; set; } = null;
        public AttendeeDto AttendeeDto { get; set; } = null;
        public TicketTypeDto TicketTypeDto { get; set; } = null;
        public ReceiptDto ReceiptDto { get; set; } = null;
        public SalesOrderDto SalesOrderDto { get; set; } = null;

    }
}
