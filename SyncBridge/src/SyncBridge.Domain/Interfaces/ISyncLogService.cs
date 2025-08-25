using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SyncBridge.Domain.Models;

namespace SyncBridge.Domain.Interfaces
{
    public interface ISyncLogService
    {
        public Task<SyncLog> CreateSyncLog(QueueModel syncData, ILogger logger);
        public Task<List<SyncLog>> GetFailedRecors();

        public Task<SyncLog> UpdateStatus(string id, string status);

        public Task<SyncLog> UpdateSyncLog(SyncLogUpdate syncLogUpdate, ILogger log);
    }
}
