using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SyncBridge.Domain.DTOs;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models.CVENT;

namespace SyncBridge.Application.UseCases;

public class SyncCVENTToCosmos
{
    private readonly ICventService _cventService;

    private readonly IEventServices _eventServices;

    // If no event found, sync from default date (evaluated at object creation)
    private readonly DateTime _defaultSyncDate = DateTime.UtcNow.AddDays(-4);

    public SyncCVENTToCosmos(ICventService cventService, IEventServices eventServices)
    {
        _cventService = cventService;
        _eventServices = eventServices;
    }

    public async Task Process(CancellationToken cancellationToken = default)
    {
        // Unified sync pipeline per module
        await SyncModuleAsync<CventEventResponse, EventEntity>(
            moduleName: "Event",
            containerName: "Events",
            map: e => e.ToEntity(),
            cancellationToken);

        await SyncModuleAsync<CVENTTicketTypeResponse, TicketType>(
            moduleName: "TicketType",
            containerName: "TicketTypes",
            map: t => t.ToEntity(),
            cancellationToken);

        await SyncModuleAsync<CVENTAttendeeResponse, Attendee>(
            moduleName: "Attendee",
            containerName: "Attendees",
            map: a => a.ToEntity(),
            cancellationToken);

        await SyncModuleAsync<CVENTContactResponse, Domain.Models.CVENT.Contact>(
            moduleName: "Contact",
            containerName: "Contacts",
            map: c => c.ToEntity(),
            cancellationToken);

        // TODO: SalesOrder (Transaction/Order) pipeline not yet implemented.
    }

    private async Task SyncModuleAsync<TSource, TEntity>(
        string moduleName,
        string containerName,
        Func<TSource, TEntity> map,
        CancellationToken cancellationToken)
        where TEntity : CventCommonEntity
    {
        //cancellationToken.ThrowIfCancellationRequested();

        // Get last sync timestamp (exclusive) or fallback
        var lastSync = await _eventServices.RetrieveRecentSyncTimestamp<TEntity>(containerName)
                       ?? _defaultSyncDate;

        // Add one second to avoid re-processing boundary record
        var fetchAfter = lastSync.AddSeconds(1);

        var sourceItems = await _cventService.FetchData<TSource>(moduleName, fetchAfter);
        if (sourceItems == null || sourceItems.Count == 0)
            return;

        //cancellationToken.ThrowIfCancellationRequested();

        var entities = sourceItems.Select(map).ToList();
        if (entities.Count == 0)
            return;

        await _eventServices.AddBulkAsync(entities, containerName);
    }
}
