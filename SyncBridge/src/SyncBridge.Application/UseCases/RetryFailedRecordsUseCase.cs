using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models;

namespace SyncBridge.Application.UseCases;

public class RetryFailedRecordsUseCase(
    IConfiguration configuration,
    ISyncLogService synclog,
    IMessageService messageService,
    IRetryService retryService,
    ILogger<RetryFailedRecordsUseCase> logger
)
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ISyncLogService _synclog = synclog;
    private readonly IMessageService _messageService = messageService;
    private readonly IRetryService _retryService = retryService;
    private readonly ILogger<RetryFailedRecordsUseCase> _logger = logger;

    private const int DEFAULT_MAX_RETRY_COUNT = 3; // Default value, can be overridden by configuration

    public async Task RetryFailedRecords()
    {
        var failedRecords = await _synclog.GetFailedRecors();

        if (failedRecords is not null and { Count: > 0 })
        {
            _logger.LogInformation("Retry records count", failedRecords.Count);
            var maxRetryCountConfig = _configuration.GetSection("SyncLogMaxRetryCount").Value;

            var maxRetryCount = string.IsNullOrEmpty(maxRetryCountConfig)
                ? DEFAULT_MAX_RETRY_COUNT
                : Convert.ToInt32(maxRetryCountConfig);

            // Send notifications for failed entries
            await ProcessFinalFailedRecords([.. failedRecords.Where(x => x.RetryCount >= maxRetryCount)]);

            // Process CMMP to Salesforce records
            await ProcessFailedEntriesBySource(failedRecords, SyncLogConstants.SourceSystem.CMMP, maxRetryCount);

            // Process Cvent to Salesforce records
            await ProcessFailedEntriesBySource(failedRecords, SyncLogConstants.SourceSystem.Cvent, maxRetryCount);

            // Process Salesforce to Cosmos records
            await ProcessFailedEntriesBySource(failedRecords, SyncLogConstants.SourceSystem.Salesforce, maxRetryCount);
        }
    }

    #region Private Methods
    private async Task ProcessFailedEntriesBySource(List<SyncLog> syncLogs, string source, int maxRetryCount)
    {
        var eligibleSyncLogs = GetSyncLogsBasedOnSource(syncLogs, source, maxRetryCount);
        if (eligibleSyncLogs is not null && eligibleSyncLogs.Count != 0)
        {
            foreach (var syncLog in syncLogs)
            {
                var latestHistory = syncLog.History
                                       .OrderByDescending(x => x.Timestamp)
                                       .FirstOrDefault();
                if (latestHistory is not null && latestHistory.ErrorCode == SyncLogConstants.ErrorCode.SF_Failed)
                {
                    syncLog.Action = SyncLogConstants.Action.Retry;
                    await _retryService.PublishEvent(syncLog);
                    // Increment the retry count in Handler
                }
                // TODO: call the CMMP/CVENT API if the ErrorCode is SF_ID_UPDATE_FAILED
            }
        }
    }

    private static List<SyncLog> GetSyncLogsBasedOnSource(List<SyncLog> syncLogs, string source, int maxRetryCount)
    {
        return
        [
            .. syncLogs.Where(x => x.Source == source && x.RetryCount < maxRetryCount).OrderBy(x => x.SyncCreatedAt),
        ];
    }

    private async Task ProcessFinalFailedRecords(List<SyncLog> syncLogs)
    {
        if (syncLogs is null || syncLogs.Count == 0)
        {
            return;
        }
        await SendNotificationForFailedEntries(syncLogs);

        foreach (var syncLog in syncLogs)
        {
            await _synclog.UpdateStatus(syncLog.id, SyncLogConstants.Status.Aborted);
        }
    }

    private async Task SendNotificationForFailedEntries(List<SyncLog> syncLogs)
    {
        if (syncLogs is null || syncLogs.Count == 0)
        {
            return;
        }
        foreach (var syncLog in syncLogs)
        {
            var history = syncLog.History?.OrderByDescending(x => x.Timestamp).First();
            if (history is not null)
            {
                Notification notification = new Notification
                {
                    syncLogId = syncLog.id,
                    Application = syncLog.Source,
                    Source =
                        syncLog.Destination == SyncLogConstants.SourceSystem.Salesforce
                            ? SyncLogConstants.SourceSystem.Cosmos
                            : SyncLogConstants.SourceSystem.Salesforce,
                    Destination = syncLog.Destination,
                    RecordName = syncLog.Module,
                    RecordId = syncLog.RecordId,
                    StartTime = syncLog.SyncLastAttempt,
                    Endtime = history.Timestamp,
                    Status = syncLog.Status,
                    Error = CleanSpecialCharacters(history.Error ?? ""),
                };
                await _messageService.PublishEvent(notification);
            }
        }
    }

    // TODO: Consider using a more robust cleaning method for special characters
    // Question: Why do we need to clean special characters? Why not just use the original string?
    public static string CleanSpecialCharacters(string input)
    {
        // Replace escaped quotes and backslashes
        string cleaned = input
            .Replace("[", "")
            .Replace("]", "")
            .Replace("{", "")
            .Replace("}", "")
            .Replace("\"", "")
            .Replace("\\n", " ");
        return cleaned;
    }
    #endregion
}
