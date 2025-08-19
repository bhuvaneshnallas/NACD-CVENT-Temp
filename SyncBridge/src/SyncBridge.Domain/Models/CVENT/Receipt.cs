using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Models.CVENT
{
    public class Receipt : CventCommonEntity
    {
        public string id { get; set; }
        public string? EventId { get; set; }
        public string? TransactionType { get; set; }
        public string? OrderTransactionType { get; set; }
        public string? TransactionNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? TransactionName { get; set; }
        public string? TransactionBatchNumber { get; set; }
        public string? AuthenticationCode { get; set; }
        public DateTime? PostedDate { get; set; }
        public List<ReceiptLines>? ReceiptLines { get; set; }
        public string? ExternalEventId { get; set; }
        public string? ExternalReceiptId { get; set; }
        public string? ExternalContactId { get; set; }
        public string? ExternalOrderId { get; set; }
        public string? Source { get; set; }
        public string? SFReceiptId { get; set; }
        public string? SFContactId { get; set; }
        public string? SFOrderId { get; set; }
        public string? PaymentMethodDescription { get; set; }
        public string? BatchId { get; set; } = "";


    }

    public class ReceiptLines
    {
        public string? ExternalReceiptItemId { get; set; }
        public string? LineDistributionID { get; set; }
        public decimal? SalePrice { get; set; }

    }
}

