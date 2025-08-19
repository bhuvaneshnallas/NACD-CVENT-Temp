using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.DTOs
{
    public class SalesForceTokenResponse
    {
        public string Access_token { get; set; }
        public string Instance_url { get; set; }
        public string Id { get; set; }
        public string Token_type { get; set; }
        public string Issued_at { get; set; }
        public string Signature { get; set; }
    }
}
