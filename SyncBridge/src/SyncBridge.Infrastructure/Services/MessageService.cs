using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models;

namespace SyncBridge.Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        private readonly EventGridPublisherClient _notificationClient;
        private readonly ILogger<MessageService> _logger;
        private readonly ISyncLogService _synclog;

        public MessageService(IConfiguration config, ILogger<MessageService> logger, ISyncLogService synclog)
        {
            var NotificationEndPoint = config["NotificationEventGrid:EndPoint"];
            var NotificationKey = config["NotificationEventGrid:Key"];
            _notificationClient = new EventGridPublisherClient(
                new Uri(NotificationEndPoint),
                new AzureKeyCredential(NotificationKey)
            );
            _synclog = synclog;
            _logger = logger;
        }

        public async Task PublishEvent(Notification notification)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));
            _logger.LogInformation("EventGrid Publish Initiated");

            var cloudEvent = new CloudEvent(
                type: $"{notification.RecordName}",
                source: $"{notification.RecordId}",
                data: BinaryData.FromObjectAsJson(notification),
                dataContentType: "application/json"
            );
            if (_notificationClient is null)
            {
                _logger.LogWarning("No EventGrid client configured for source: {Source}", notification.Source);
                return;
            }
            var result = await _notificationClient.SendEventAsync(cloudEvent);
            _logger.LogInformation("Notification Push : {Status}", result.Status == 200 ? "Completed" : "Failed");
        }
    }
}
