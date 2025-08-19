using Azure.Messaging;
using SyncBridge.Domain.Interfaces;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace SyncBridge.Application.UseCases
{
    public class SyncEventGridToSalesForce
    {
        private readonly IEventGridService _eventGridService;
        private readonly ISalesforceService _salesforceService;

        public SyncEventGridToSalesForce(IEventGridService eventGridService,ISalesforceService salesforceService)
        {
            _eventGridService = eventGridService;// ?? new EventGridService();
            _salesforceService = salesforceService;
        }

        public async Task EventGridToSalesForce(CloudEvent cloudEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                    object data = await _eventGridService.GetEventGridData(cloudEvent);
                    await _salesforceService.SalesForcePost(data);
            }
            catch (Exception ex)
            {
                // TODO: log exception
                throw;
            }
        }

    }
}
