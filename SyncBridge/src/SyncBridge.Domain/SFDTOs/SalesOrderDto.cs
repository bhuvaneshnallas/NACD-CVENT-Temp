
namespace CventSalesforceSyncApi.Domain.DTO
{
    public class SalesOrderDto
    {
        public string Id { get; set; }
        public string ExternalOrderId { get; set; }
        public string TransactionType { get; set; }
        public string TransactionNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? PostingStatus { get; set; }
        public string Entity { get; set; }
        public List<SalesOrderLineDto> SalesOrderLines { get; set; }
        public string SFContactId { get; set; }
        public string SFEventId { get; set; }
        public string? SFOrderId { get; set; }
        public string BatchId { get; set; } = Guid.NewGuid().ToString();

    }

    public class SalesOrderLineDto
    {
        public decimal? AmountOrdered { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? AmountDue { get; set; }
        public string? ExternalTicketTypeId { get; set; }
        public string? ExternalOrderItemId { get; set; }

    }

}
