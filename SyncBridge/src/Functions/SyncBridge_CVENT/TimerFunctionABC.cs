using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SyncBridge.Application.UseCases;
using System;

namespace SyncBridge_CVENT;

public class TimerFunctionABC
{
    private readonly ILogger _logger;
    private readonly SyncCVENTToCosmos _syncCVENTToCosmos;

    public TimerFunctionABC(ILoggerFactory loggerFactory, SyncCVENTToCosmos syncCVENTToCosmos)
    {
        _logger = loggerFactory.CreateLogger<TimerFunctionABC>();
        _syncCVENTToCosmos = syncCVENTToCosmos;
    }

    [Function("TimerFunctionABC")]
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        try
        {
            await _syncCVENTToCosmos.Process();

        }
        catch (Exception e)
        {

            throw;
        }



        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);
        
        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }
    }
}