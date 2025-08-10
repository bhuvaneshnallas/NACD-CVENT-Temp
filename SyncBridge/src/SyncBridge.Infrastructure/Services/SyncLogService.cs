using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models;
using SyncBridge.Domain.Utility;
using SyncBridge.Infrastructure.Data;

namespace SyncBridge.Infrastructure.Services
{
    public class SyncLogService : ISyncLogService
    {
        private readonly IDbContextFactory<SyncLogDBContext> _contextFactory;
        private readonly IConfiguration _configuration;

        public SyncLogService(IDbContextFactory<SyncLogDBContext> contextFactory, IConfiguration configuration)
        {
            _contextFactory = contextFactory;
            _configuration = configuration;
        }

        public async Task<SyncLog> CreateSyncLog(QueueModel syncData, ILogger logger)
        {
            if (syncData == null)
                throw new ArgumentNullException(nameof(syncData));
            using var context = _contextFactory.CreateDbContext();
            logger.LogInformation($@"synclog save start {syncData.module + syncData.recordId}");
            List<History> histories = new List<History>();
            History history = new History
            {
                id = Guid.NewGuid().ToString(),
                HandlerName = SyncLogConstants.SourceSystem.Salesforce,
                Message = "",
                Status = SyncLogConstants.Status.InProgress,
                Error = string.Empty,
                ErrorCode = string.Empty,
                Timestamp = SyncConstants.getCurrentESTDateTime(),
            };
            histories.Add(history);
            SyncLog syncLog = new SyncLog();
            syncLog.id = Guid.NewGuid().ToString();
            syncLog.Source = SyncLogConstants.SourceSystem.Salesforce;
            syncLog.Destination = SyncLogConstants.SourceSystem.CMMP;
            syncLog.RecordId = syncData.id;
            syncLog.Module = syncData.module;
            syncLog.Status = SyncLogConstants.Status.InProgress;
            syncLog.SyncLastAttempt = SyncConstants.getCurrentESTDateTime();
            syncLog.SyncCreatedAt = SyncConstants.getCurrentESTDateTime();
            syncLog.RetryCount = 0;
            syncLog.ModuleCreatedAt = syncData.moduleCreatedAt;
            syncLog.ModuleCreatedBy = syncData.moduleCreatedBy;
            syncLog.ModuleUpdatedAt = syncData.moduleUpdatedAt;
            syncLog.ModuleUpdatedBy = syncData.moduleUpdatedBy;
            syncLog.Action = syncData.action;
            syncLog.History = histories;
            try
            {
                await context.SyncLog.AddAsync(syncLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error saving SyncLog {syncLog.id}: {ex.Message}", ex);
            }
            logger.LogInformation($@"synclog save end {syncLog.id}");
            return syncLog;
        }

        public async Task<SyncLog> UpdateSyncLog(
            string syncLogId,
            ILogger log,
            string handlerName,
            string status,
            string error,
            string errorCode
        )
        {
            using var context = _contextFactory.CreateDbContext();
            var syncLog = context.SyncLog.FirstOrDefault(x => x.id == syncLogId);
            if (syncLog != null)
            {
                log.LogInformation($@"synclog save start {syncLogId}");
                syncLog.Status = status;
                var history = new History
                {
                    id = Guid.NewGuid().ToString(),
                    HandlerName = handlerName,
                    Message = string.Empty,
                    Status = status,
                    Error = error,
                    ErrorCode = errorCode,
                    Timestamp = SyncConstants.getCurrentESTDateTime(),
                };
                syncLog.History?.Add(history);
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    log.LogError($"Error saving SyncLog {syncLogId}: {ex.Message}", ex);
                }
                log.LogInformation($@"synclog save end {syncLogId}");
            }
            return syncLog;
        }

        public async Task<SyncLog?> UpdateStatusAsync(string id, string status)
        {
            await using var context = _contextFactory.CreateDbContext();

            var syncLog = await context.SyncLog.FirstOrDefaultAsync(x => x.id == id);

            if (syncLog is null)
                return null;

            syncLog.Status = status;
            await context.SaveChangesAsync();

            return syncLog;
        }

        public async Task<List<SyncLog>> GetFailedRecors()
        {
            using var context = _contextFactory.CreateDbContext();

            List<SyncLog> syncLogs = new List<SyncLog>();
            syncLogs = await context
                .SyncLog.Where(x =>
                    x.Status == SyncLogConstants.Status.Failed
                //&& x.Source == SyncLogConstants.SourceSystem.CMMP
                //&& x.Module == "SalesOrder"
                )
                .ToListAsync();
            return syncLogs;
        }

        public async Task<SyncLog> UpdateStatus(string id, string status)
        {
            using var context = _contextFactory.CreateDbContext();
            var syncLogData = await context.SyncLog.FindAsync(id);
            if (syncLogData == null)
            {
                return null;
            }
            syncLogData.Status = status;
            context.SyncLog.Update(syncLogData);
            await context.SaveChangesAsync();
            return syncLogData;
        }
    }
}
