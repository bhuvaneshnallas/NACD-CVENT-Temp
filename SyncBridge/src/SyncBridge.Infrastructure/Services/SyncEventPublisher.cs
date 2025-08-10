using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Configuration;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models;

namespace SyncBridge.Infrastructure.Services
{
    public class SyncEventPublisher : ISyncEventPublisher
    {
        private readonly IConfiguration _config;

        public SyncEventPublisher(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> PublishAsync<T>(T eventData, string id, string module, string eventType, string source)
        {
            EventGridPublisherClient? client = GetClient(source, _config);

            var cloudEvent = new CloudEvent(
                type: $"{module} {SyncLogConstants.Action.Retry}",
                source: id,
                data: BinaryData.FromObjectAsJson(eventData),
                dataContentType: "application/json"
            );

            var result = await client.SendEventAsync(cloudEvent);
            return result.Status == 200;
        }

        private EventGridPublisherClient GetClient(string source, IConfiguration configuration)
        {
            // Map source system to endpoint and key configuration keys
            (string endPointKey, string keyKey) = source switch
            {
                SyncLogConstants.SourceSystem.CMMP => ("CMMPToSfEventGrid:EndPoint", "CMMPToSfEventGrid:Key"),
                SyncLogConstants.SourceSystem.Cvent => ("CventToSFEventGrid:EndPoint", "CventToSFEventGrid:Key"),
                SyncLogConstants.SourceSystem.Salesforce => ("SfToCMMPEventGrid:EndPoint", "SfToCMMPEventGrid:Key"),
                _ => throw new ArgumentException($"Unsupported source system: {source}"),
            };

            string? endPoint = configuration[endPointKey];
            string? key = configuration[keyKey];

            if (string.IsNullOrWhiteSpace(endPoint) || string.IsNullOrWhiteSpace(key))
                throw new ArgumentException($"EventGrid configuration for {source} is not set properly.");

            return new EventGridPublisherClient(new Uri(endPoint), new AzureKeyCredential(key));
        }
    }
}
