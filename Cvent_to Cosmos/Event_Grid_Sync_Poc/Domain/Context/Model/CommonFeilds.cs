namespace Event_Grid_Sync_Poc.Domain.Context.Model
{
    public class CommonFields
    {
        public DateTime? CreatedDT { get; set; }
        public DateTime? ModifiedDT { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public bool? IsProcessed { get; set; }
    }
}
