using SyncBridge.Domain.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SyncBridge.Domain.Models.CVENT;

public class TicketType : CventCommonEntity
{
    // Add this property to ensure type is always set to identified value

    //private string? _type;

    //public string? type
    //{
    //    get { return _type; }
    //    set
    //    {
    //        if (string.IsNullOrEmpty(value))
    //        {
    //            _type = nameof(TicketType);
    //        }
    //        else
    //        {
    //            _type = value;
    //        }
    //    }
    //}

    public string? EventId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductId { get; set; }
    public string? FeeId { get; set; }
    public decimal? Price { get; set; }
    public string? IncomeAccount { get; set; }
    public string? ExternalTicketTypeId { get; set; }
    public string? ExternalEventId { get; set; }
    public string? Source { get; set; }
    public string? SFTicketTypeId { get; set; }
    public string? BatchId { get; set; }
    public string? ProcessQueueId { get; set; }
}
