using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Models.CVENT
{
    public class BaseModel
    {
        public DateTime? CreatedDT { get; set; }
        public DateTime? ModifiedDT { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
