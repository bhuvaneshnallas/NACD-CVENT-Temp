using Azure.Messaging;
using SyncBridge.Domain.Models.DTOs;

namespace SyncBridge.Domain.Interfaces
{
    public interface ISalesforceService { 
        public Task<HttpResponseMessage> SalesforceApiCall(dynamic mappingData, string destinationSfUrl);
    }
}
