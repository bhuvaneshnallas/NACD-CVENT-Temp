using SyncBridge.Domain.DTOs;

namespace SyncBridge.Domain.Interfaces;

public interface ICventService
{
    Task<string> GetToken();
    Task<List<T>> FetchData<T>(string moduleName, DateTime fetchAfter);
}
