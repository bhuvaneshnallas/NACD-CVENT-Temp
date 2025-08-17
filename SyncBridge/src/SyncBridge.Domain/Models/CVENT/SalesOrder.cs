using System.ComponentModel.DataAnnotations;

namespace SyncBridge.Domain.Models.CVENT;

public class SalesOrder : CventCommonEntity
{
    public string? EventId { get; set; }
    public string? TransactionType { get; set; }
    public string? TransactionNumber { get; set; }
    public DateTime? TransactionDate { get; set; }
    public string? PostingStatus { get; set; }
    public string? Entity { get; set; }
    public string? ExternalEventId { get; set; }
    public string? ExternalContactId { get; set; }
    public string? ExternalOrderId { get; set; }
    public string? Source { get; set; }
    public string? PaymentType { get; set; }
    public string? SFOrderId { get; set; }
    public string? BatchId { get; set; }
    public string? ProcessQueueId { get; set; }
    public List<SalesOrderLine>? SalesOrderLines { get; set; }
}

public class SalesOrderLine
{
    public string? ExternalOrderItemId { get; set; }
    public string? ExternalTicketTypeId { get; set; }
    public decimal? AmountOrdered { get; set; }
    public decimal? AmountPaid { get; set; }
    public decimal? AmountDue { get; set; }
}
