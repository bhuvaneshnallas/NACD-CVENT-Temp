using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models.CVENT;

namespace SyncBridge.Infrastructure.Data.Repository;

public class EventsDbRepo : IEventsDbRepo
{
    private readonly CosmosClient _cosmosClient;
    private readonly IConfiguration _configuration;

    public EventsDbRepo(CosmosClient cosmosClient, IConfiguration configuration)
    {
        _cosmosClient = cosmosClient;
        _configuration = configuration;
    }

    public async Task AddBulkAsync<T>(List<T> items, string containerName)
        where T : CventCommonEntity
    {
        Database database = _cosmosClient.GetDatabase(_configuration["EventDB:DatabaseName"]);
        Container container = database.GetContainer(containerName);

        List<Task> tasks = new List<Task>();

        foreach (var item in items)
        {
            // string pkValue = (string)item.GetType().GetProperty("id").GetValue(item, null);
            tasks.Add(container.UpsertItemAsync(item, new PartitionKey(item.id)));
        }

        // Execute in parallel
        await Task.WhenAll(tasks);
    }

    public async Task<List<T>> GetAllAsync<T>(
        string containerName,
        string? whereClause = null,
        Dictionary<string, object>? parameters = null
    )
        where T : CventCommonEntity
    {
        Database database = _cosmosClient.GetDatabase(_configuration["EventDB:DatabaseName"]);
        Container container = database.GetContainer(containerName);

        QueryDefinition query;

        if (string.IsNullOrWhiteSpace(whereClause))
        {
            query = new QueryDefinition("SELECT * FROM c");
        }
        else
        {
            // Expect caller to provide a WHERE clause without the leading WHERE keyword, e.g. "c.Status = @status"
            query = new QueryDefinition($"SELECT * FROM c WHERE {whereClause}");

            if (parameters != null)
            {
                foreach (var kv in parameters)
                {
                    // Cosmos parameter names must be prefixed with @ when used in the clause; caller should use @name in whereClause
                    query = query.WithParameter("@" + kv.Key, kv.Value);
                }
            }
        }

        FeedIterator<T> iterator = container.GetItemQueryIterator<T>(query);

        List<T> results = new List<T>();

        while (iterator.HasMoreResults)
        {
            FeedResponse<T> response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task UpdateAsync<T>(string containerName, List<T> items)
        where T : CventCommonEntity
    {
        Database database = _cosmosClient.GetDatabase(_configuration["EventDB:DatabaseName"]);
        Container container = database.GetContainer(containerName);

        List<Task> tasks = new List<Task>();

        foreach (var item in items)
        {
            tasks.Add(container.UpsertItemAsync(item, new PartitionKey(item.id)));
        }

        await Task.WhenAll(tasks);
    }

    public async Task<T?> GetLatestRecordFromContainerAsync<T>(string containerName)
        where T : CventCommonEntity
    {
        Database database = _cosmosClient.GetDatabase(_configuration["EventDB:DatabaseName"]);
        Container container = database.GetContainer(containerName);

        QueryDefinition query = new QueryDefinition("SELECT * FROM c ORDER BY c.CreatedDT DESC");

        FeedIterator<T> iterator = container.GetItemQueryIterator<T>(query);

        if (iterator.HasMoreResults)
        {
            FeedResponse<T> response = await iterator.ReadNextAsync();
            return response.FirstOrDefault();
        }

        return null;
    }
}
