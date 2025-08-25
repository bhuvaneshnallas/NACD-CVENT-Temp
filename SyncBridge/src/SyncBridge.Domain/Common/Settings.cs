using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Common
{
    public class Settings
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string? DisplayTest { get; set; }

        public string? Description { get; set; }
    }
}
