using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SyncBridge.Application.UseCases;

namespace SyncBridge_CVENT;

public class Function1
{
    private readonly ILogger<Function1> _logger;
    private readonly SyncCVENTToCosmos _syncCVENTToCosmos;

    public Function1(ILogger<Function1> logger, SyncCVENTToCosmos syncCVENTToCosmos)
    {
        _logger = logger;
        _syncCVENTToCosmos = syncCVENTToCosmos;
    }

    [Function("Function1")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        try
        {
            await _syncCVENTToCosmos.Process();

        }
        catch (Exception e)
        {

            throw;
        }



        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}
