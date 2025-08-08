using Azure.Messaging;
using Azure.Messaging.EventGrid.SystemEvents;
using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Cors;
using System.ComponentModel.DataAnnotations;

[EnableCors]
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly CventEventRepository _repository;
    private static int dataCount = 0;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, CventEventRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }


    //[HttpOptions("Options")]
    //public IActionResult Options()
    //{
    //    Response.Headers.Add("WebHook-Allowed-Origin", "*"); 
    //    return Ok();
    //}

    [HttpPost("PostEvent")]
    public async Task<IActionResult> PostAsync([FromBody] JsonElement rawEvents)
    {
        try
        {
            int current = Interlocked.Increment(ref dataCount);
            var cloudEvents = CloudEvent.ParseMany(BinaryData.FromString(rawEvents.GetRawText()));

            foreach (var cloudEvent in cloudEvents)
            {
                await _repository.HandleEventAsync(cloudEvent, current);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in PostAsync.");
            return StatusCode(500, "Internal Server Error.");
        }
    }

}
