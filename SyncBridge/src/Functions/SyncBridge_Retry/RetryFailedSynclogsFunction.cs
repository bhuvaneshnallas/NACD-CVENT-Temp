using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SyncBridge_Retry;

public class RetryFailedSynclogsFunction
{
    private readonly ILogger _logger;
    private readonly RetryFailedRecordsUseCase _retryFailedRecords;
    private readonly IConfiguration _configuration;

    public RetryFailedSynclogsFunction(
        ILoggerFactory loggerFactory,
        RetryFailedRecordsUseCase retryFailedRecords,
        IConfiguration configuration
    )
    {
        _logger = loggerFactory.CreateLogger<RetryFailedSynclogsFunction>();
        this._retryFailedRecords = retryFailedRecords;
        this._configuration = configuration;
    }

    [Function("RetryFailedSynclogsFunction")]
    public void Run([TimerTrigger("%SyncLogRetryInterval%")] TimerInfo myTimer)
    {
        //   */30 * * * * *    -- Every 30 seconds
        //   0 */5 * * * *     -- Every 5 minute
        //   0 0 */1 * * *     -- Every 1 hour
        //   0 30 4 * * *      -- Every day 4:30 AM
        //   SyncLogRetryInterval configured in appsettings.json
        _logger.LogInformation("Retry function executed at: {executionTime}", DateTime.Now);
        try
        {
            var syncLog = _retryFailedRecords.RetryFailedRecords();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrying failed records: {Message}", ex.Message);
        }
    }
}
