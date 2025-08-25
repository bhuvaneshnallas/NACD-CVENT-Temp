using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models;
using SyncBridge.Domain.Utility;
using SyncBridge.Infrastructure.Data;
using SyncBridge.Domain.Models;

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
            syncLog.Source = SyncLogConstants.SourceSystem.Cvent;
            syncLog.Destination = SyncLogConstants.SourceSystem.Salesforce;
            syncLog.RecordId = syncData.id;
            syncLog.Module = syncData.module;
            syncLog.Status = SyncLogConstants.Status.Ready;
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

        public async Task<SyncLog> UpdateSyncLog(SyncLogUpdate syncLogUpdate, ILogger log)
        {
            log.LogInformation($@"synclog Update Initiated {syncLogUpdate.id}");
            using var context = _contextFactory.CreateDbContext();
            var syncLog = await context.SyncLog.FirstOrDefaultAsync(x => x.id == syncLogUpdate.id);
            if (syncLog != null)
            {
                // Update main status
                syncLog.Status = syncLogUpdate.Status;
                syncLog.Destination = syncLogUpdate.IsCventContact ? SyncLogConstants.SourceSystem.Cvent : syncLog.Destination;
                syncLog.RetryCount = syncLogUpdate.Status == SyncLogConstants.Status.Retry_Inititated ? syncLog.RetryCount + 1 : syncLog.RetryCount;
                var history = new History
                {
                    id = Guid.NewGuid().ToString(),
                    HandlerName = syncLogUpdate.HandlerName,
                    Message = string.Empty,
                    Status = syncLogUpdate.Status,
                    Error = syncLogUpdate.Error,
                    ErrorCode = syncLogUpdate.ErrorCode,
                    Timestamp = SyncLogConstants.getCurrentESTDateTime()
                };
                syncLog.History?.Add(history);
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    log.LogError($"Error saving SyncLog: {ex.Message}", ex);
                }
                log.LogInformation($@"Synclog Update Completed");
            }
            return syncLog;
        }


    }
}
