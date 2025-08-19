
namespace CventSalesforceSyncApi.Domain.DTO
{
    public class TicketTypeDto 
    {
        public string? id { get; set; }
        public string ExternalTicketTypeId { get; set; }
        public string ExternalEventId { get; set; }
        public string ProductName { get; set; }
        public string ProductId { get; set; }
        public string FeeId { get; set; }
        public string Price { get; set; }
        public string? IncomeAccount { get; set; }
        public string? SFEventId { get; set; }
        public string? SFTicketTypeId { get; set; } = "";
        public string BatchId { get; set; } = Guid.NewGuid().ToString();

        public DateTime? CreatedDT { get; set; }
        public DateTime? ModifiedDT { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
