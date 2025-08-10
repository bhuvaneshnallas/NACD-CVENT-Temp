using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncBridge.Domain.Models;

namespace SyncBridge.Domain.Interfaces
{
    public interface IRetryService
    {
        public Task PublishEvent(SyncLog model);
    }
}
