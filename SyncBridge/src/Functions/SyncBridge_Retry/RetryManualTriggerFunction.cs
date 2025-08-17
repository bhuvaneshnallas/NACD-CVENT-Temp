namespace SyncBridge_Retry;

public class RetryManualTriggerFunction(
    ILogger<RetryManualTriggerFunction> logger,
    RetryFailedRecordsUseCase retryFailedRecords
)
{
    private readonly ILogger<RetryManualTriggerFunction> _logger = logger;
    private readonly RetryFailedRecordsUseCase _retryFailedRecords = retryFailedRecords;

    [Function("RetryManualTriggerFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        try
        {
            var syncLog = _retryFailedRecords.RetryFailedRecords();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrying failed records: {Message}", ex.Message);
        }
        return new OkObjectResult("Retry operation completed successfully.");
    }
}
