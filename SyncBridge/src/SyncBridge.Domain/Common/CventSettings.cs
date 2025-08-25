using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Common
{
    public class CventSettings
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public List<Settings>? Data { get; set; }

    }
}
