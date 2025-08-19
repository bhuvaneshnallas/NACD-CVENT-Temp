// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System;
using System.Text.Json;
using Azure.Messaging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SyncBridge.Application.UseCases;
using SyncBridge.Infrastructure.Services;

namespace SyncBridge_CVENT;

public class EventGridReciever
{
    private readonly ILogger<EventGridReciever> _logger;
    private readonly SyncEventGridToSalesForce _syncEventGridToSalesForce;

    public EventGridReciever(ILogger<EventGridReciever> logger,SyncEventGridToSalesForce syncEventGridToSalesForce)
    {
        _logger = logger;

        //TODO: DI
        _syncEventGridToSalesForce = syncEventGridToSalesForce;
    }

    [Function(nameof(EventGridReciever))]
    public async Task Run([EventGridTrigger] CloudEvent cloudEvent)
    {
        //_logger.LogInformation("Event type: {type}, Event subject: {subject}", cloudEvent.Type, cloudEvent.Subject);
         await _syncEventGridToSalesForce.EventGridToSalesForce(cloudEvent);
        
    }
}