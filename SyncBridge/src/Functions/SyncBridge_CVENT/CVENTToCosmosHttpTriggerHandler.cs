using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SyncBridge.Application.UseCases;

namespace SyncBridge_CVENT;

public class CVENTToCosmosHttpTriggerHandler
{
    private readonly SyncCVENTToCosmos _syncCVENTToCosmos;
    private readonly ILogger<CVENTToCosmosHttpTriggerHandler> _logger;

    public CVENTToCosmosHttpTriggerHandler(SyncCVENTToCosmos syncCVENTToCosmos, ILogger<CVENTToCosmosHttpTriggerHandler> logger)
    {
        _syncCVENTToCosmos = syncCVENTToCosmos;
        _logger = logger;
    }

    [Function("CVENTToCosmosHttpTrigger")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        try
        {
            await _syncCVENTToCosmos.Process();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the CVENT to Cosmos sync.");
        }

        return new OkObjectResult("CVENT to Cosmos sync completed successfully.");
    }
}
