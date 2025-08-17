using System.Collections.Generic;
using SyncBridge.Domain.Models.CVENT;

namespace SyncBridge.Domain.Interfaces;

public interface IEventsDbRepo
{
    Task AddBulkAsync<T>(List<T> items, string containerName)
        where T : CventCommonEntity;

    /// <summary>
    /// Retrieve all items from a container. Optionally provide a WHERE clause (without the "WHERE" keyword)
    /// and a dictionary of named parameters (keys without the '@' prefix).
    /// Example: whereClause = "c.Status = @status AND c.CreatedDT &gt; @since" with parameters { { "status", "Active" }, { "since", someDate } }
    /// </summary>
    Task<List<T>> GetAllAsync<T>(
        string containerName,
        string? whereClause = null,
        Dictionary<string, object>? parameters = null
    )
        where T : CventCommonEntity;

    Task UpdateAsync<T>(string containerName, List<T> items)
        where T : CventCommonEntity;

    Task<T?> GetLatestRecordFromContainerAsync<T>(string containerName)
        where T : CventCommonEntity;
}
