using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models.CVENT;

namespace SyncBridge.Infrastructure.Services;

public class EventServices : IEventServices
{
    private readonly IEventsDbRepo _eventsDbRepo;

    public EventServices(IEventsDbRepo eventsDbRepo)
    {
        _eventsDbRepo = eventsDbRepo;
    }

    public async Task AddBulkAsync<T>(List<T> items, string containerName)
        where T : CventCommonEntity
    {
        await _eventsDbRepo.AddBulkAsync(items, containerName);
    }

    public async Task<List<T>> GetRecordsForSync<T>(string containerName)
        where T : CventCommonEntity
    {
        string whereClause = "c.SentForSync = @SentForSync";
        var parameters = new Dictionary<string, object> { { "SentForSync", false } };

        return await _eventsDbRepo.GetAllAsync<T>(containerName, whereClause, parameters);
    }

    public async Task MarkRecordsAsSynced<T>(string containerName, List<T> records)
        where T : CventCommonEntity
    {
        foreach (var record in records)
        {
            record.SentForSync = true;
        }

        await _eventsDbRepo.UpdateAsync(containerName, records);
    }

    public async Task<DateTime?> RetrieveRecentSyncTimestamp<T>(string containerName)
        where T : CventCommonEntity
    {
        var latestRecord = await _eventsDbRepo.GetLatestRecordFromContainerAsync<T>(containerName);
        return latestRecord?.ModifiedDT;
    }
}
