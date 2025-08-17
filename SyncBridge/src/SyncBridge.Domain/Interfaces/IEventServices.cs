using SyncBridge.Domain.Models.CVENT;

namespace SyncBridge.Domain.Interfaces;

public interface IEventServices
{
    Task AddBulkAsync<T>(List<T> items, string containerName)
        where T : CventCommonEntity;
    Task<List<T>> GetRecordsForSync<T>(string containerName)
        where T : CventCommonEntity;
    Task MarkRecordsAsSynced<T>(string containerName, List<T> records)
        where T : CventCommonEntity;
    Task<DateTime?> RetrieveRecentSyncTimestamp<T>(string containerName)
        where T : CventCommonEntity;
}
