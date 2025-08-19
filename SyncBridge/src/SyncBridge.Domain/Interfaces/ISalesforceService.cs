using SyncBridge.Domain.Models.DTOs;

namespace SyncBridge.Domain.Interfaces
{
    public interface ISalesforceService { 
        public Task<string> SalesForcePost<T>(T entity);
    }
}
