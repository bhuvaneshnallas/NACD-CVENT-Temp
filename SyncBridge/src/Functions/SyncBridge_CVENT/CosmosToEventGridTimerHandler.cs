using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SyncBridge_CVENT;

public class CosmosToEventGridTimerHandler
{
    private readonly ILogger _logger;

    public CosmosToEventGridTimerHandler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CosmosToEventGridTimerHandler>();
    }

    [Function("CosmosToEventGridTimerTrigger")]
    public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);
        
        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }
    }
}