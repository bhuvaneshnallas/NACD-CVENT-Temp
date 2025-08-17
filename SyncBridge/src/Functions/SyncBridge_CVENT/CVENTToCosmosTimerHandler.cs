using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SyncBridge.Application.UseCases;

namespace SyncBridge_CVENT;

public class CVENTToCosmosTimerHandler
{
    private readonly ILogger _logger;
    private readonly SyncCVENTToCosmos _syncCVENTToCosmos;

    public CVENTToCosmosTimerHandler(SyncCVENTToCosmos syncCVENTToCosmos, ILoggerFactory loggerFactory)
    {
        _syncCVENTToCosmos = syncCVENTToCosmos;
        _logger = loggerFactory.CreateLogger<CVENTToCosmosTimerHandler>();

    }

    [Function("CVENTToCosmosTimerTrigger")]
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);

        if (myTimer.ScheduleStatus is not null)
        {
            try
            {
                // Temporarily disabled for manual testing
                //await _syncCVENTToCosmos.Process();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the CVENT to Cosmos sync.");
            }
        }
    }
}
