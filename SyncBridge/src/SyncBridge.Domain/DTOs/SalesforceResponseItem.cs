using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.DTOs
{
    public class SalesforceResponseItem
    {
        public string BatchId { get; set; }
        public string SfId { get; set; }
        public string Id { get; set; }
    }
}
