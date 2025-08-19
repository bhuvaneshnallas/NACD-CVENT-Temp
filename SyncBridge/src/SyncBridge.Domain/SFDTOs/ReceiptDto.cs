namespace CventSalesforceSyncApi.Domain.DTO
{
    public class ReceiptDto
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
        public string? ExternalReceiptId { get; set; }
        public string? ExternalOrderId { get; set; }
        public string? SFContactId { get; set; }
        public string? SFReceiptId { get; set; }
        public string? SFOrderId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentMethodDescription { get; set; }
        public string BatchId { get; set; } = Guid.NewGuid().ToString();

        public List<ReceiptLine> ReceiptLines { get; set; } = new List<ReceiptLine>();
    }
    public class ReceiptLine
    {
        public string? ExternalReceiptItemId { get; set; }
        public string? LineDistributionID { get; set; }
        public decimal? SalePrice { get; set; }

    }
}
