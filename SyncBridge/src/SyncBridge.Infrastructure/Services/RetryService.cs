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
    public class RetryService : IRetryService
    {
        private readonly EventGridPublisherClient _cmmpToSfClient;
        private readonly EventGridPublisherClient _sfToCMMPClient;
        private readonly EventGridPublisherClient _cvenToSfClient;
        private readonly ILogger<RetryService> _logger;

        public RetryService(IConfiguration config, ILogger<RetryService> logger)
        {
            var cmmpToSfEndPoint = config["CMMPToSfEventGrid:EndPoint"];
            var cmmpToSfKey = config["CMMPToSfEventGrid:Key"];

            var sfToCMMPEndPoint = config["SfToCMMPEventGrid:EndPoint"];
            var sfToCMMPKey = config["SfToCMMPEventGrid:Key"];

            var CvenToSfEndPoint = config["CventToSFEventGrid:EndPoint"];
            var CvenToSfKey = config["CventToSFEventGrid:Key"];

            _cmmpToSfClient = new EventGridPublisherClient(
                new Uri(cmmpToSfEndPoint),
                new AzureKeyCredential(cmmpToSfKey)
            );
            _sfToCMMPClient = new EventGridPublisherClient(
                new Uri(sfToCMMPEndPoint),
                new AzureKeyCredential(sfToCMMPKey)
            );
            _cvenToSfClient = new EventGridPublisherClient(
                new Uri(CvenToSfEndPoint),
                new AzureKeyCredential(CvenToSfKey)
            );
            _logger = logger;
        }

        public async Task PublishEvent(SyncLog syncLog)
        {
            if (syncLog == null)
                throw new ArgumentNullException(nameof(syncLog));
            _logger.LogInformation("EventGrid Publish Initiated");

            var cloudEvent = new CloudEvent(
                type: $"{syncLog.Module} {SyncLogConstants.Action.Retry}",
                source: $"{syncLog.id}",
                data: BinaryData.FromObjectAsJson(syncLog),
                dataContentType: "application/json"
            );

            EventGridPublisherClient? client = syncLog.Source switch
            {
                SyncLogConstants.SourceSystem.CMMP => _cmmpToSfClient,
                SyncLogConstants.SourceSystem.Cvent => _cvenToSfClient,
                SyncLogConstants.SourceSystem.Salesforce => _sfToCMMPClient,
                _ => null,
            };
            if (client is null)
            {
                _logger.LogWarning("No EventGrid client configured for source: {Source}", syncLog.Source);
                return;
            }
            var result = await client.SendEventAsync(cloudEvent);
            _logger.LogInformation("EventGrid Publish: {Status}", result.Status == 200 ? "Completed" : "Failed");
        }
    }
}
