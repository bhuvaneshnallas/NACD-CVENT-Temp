using System.Text;
using System.Text.Json;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Configuration;
using SyncBridge.Domain.Models;

namespace SyncBridge_Retry;

public class NotificationFunction
{
    private readonly ILogger<NotificationFunction> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly NotificationUseCase _notification;

    public NotificationFunction(
        ILogger<NotificationFunction> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        NotificationUseCase notification
    )
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _notification = notification;
    }

    [Function(nameof(NotificationFunction))]
    public async Task RunAsync([EventGridTrigger] CloudEvent cloudEvent)
    {
        _logger.LogInformation("Notification Function Initiated");

        try
        {
            var Notification = JsonSerializer.Deserialize<Notification>(cloudEvent.Data);

            if (Notification != null)
            {
                await _notification.SendNotification(Notification, _httpClient, _configuration, _logger);
            }
            _logger.LogWarning("Deserialized SyncFailure object is null.");
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in NotificationFunction.");
        }
    }
}
