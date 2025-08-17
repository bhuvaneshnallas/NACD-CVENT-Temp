namespace SyncBridge_Retry;

public class CalculatorFunction(CalculatorUseCase calculatorUseCase, ILogger<CalculatorFunction> logger)
{
    private readonly CalculatorUseCase _calculatorUseCase = calculatorUseCase;
    private readonly ILogger _logger = logger;

    [Function("CalculatorFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "calculator")] HttpRequest req
    )
    {
        var requestBody = await req.ReadFromJsonAsync<CalculatorRequest>();
        if (requestBody == null)
        {
            _logger.LogError("Invalid request body received.");
            return new BadRequestObjectResult("Invalid request body.");
        }

        var result = _calculatorUseCase.Execute(requestBody);
        return new OkObjectResult(result);

        /*
         Function - CloudEvent - >
         UseCase -> All logics will be here
            Fetch the Failed logs ->
                If entry exceeds retry count
                    Send notification
                    Make the status as ABORTED
                If Error Code -> SF-FAILED: Based on source and destination pass the notification to event grid
                If Error Code -> SF-ID_Fssss: Directly call the destination API and update the salesforceID (Get salesfource from the history)
            Increment the retry count
        For API Call's
            Interface
            Service
         */
    }
}
