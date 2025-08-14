using Microsoft.EntityFrameworkCore;
using SyncBridge.Domain.DTOs;
using SyncBridge.Domain.Interfaces;

namespace SyncBridge.Application.UseCases;

public class SyncCVENTToCosmos
{
    private readonly ICventService _cventService;
    private readonly IEventsDbContext _eventsDbContext;

    // If no event found, sync from default date (evaluated at object creation)
    private readonly DateTime _defaultSyncDate = DateTime.UtcNow.AddDays(-10);

    public SyncCVENTToCosmos(ICventService cventService, IEventsDbContext eventsDbContext)
    {
        _cventService = cventService;
        _eventsDbContext = eventsDbContext;
    }

    public async Task Process(CancellationToken cancellationToken = default)
    {
        // Process Events
        var lastModifiedEvents = GetLastModifiedDate(_eventsDbContext.Events, e => e.ModifiedDT ?? DateTime.MinValue);
        var cventEvents = await _cventService.FetchData<CventEventResponse>("Event", lastModifiedEvents);

        // Map Cvent events to Cosmos entities
        var eventEntities = cventEvents.Select(e => e.ToEntity()).ToList();
        await _eventsDbContext.Events.AddRangeAsync(eventEntities, cancellationToken);
        await _eventsDbContext.SaveChangesAsync(cancellationToken);

        //Process TicketTypes
        var lastModifiedTicketTypes = GetLastModifiedDate(_eventsDbContext.TicketTypes, t => t.ModifiedDT ?? DateTime.MinValue);
        var cventTicketTypes = await _cventService.FetchData<CVENTTicketTypeResponse>("TicketType", lastModifiedTicketTypes);

        // Map Cvent ticket types to Cosmos entities
        var ticketTypeEntities = cventTicketTypes.Select(t => t.ToEntity()).ToList();
        await _eventsDbContext.TicketTypes.AddRangeAsync(ticketTypeEntities, cancellationToken);
        await _eventsDbContext.SaveChangesAsync(cancellationToken);

        //Process Attendees
        var lastModifiedAttendees = GetLastModifiedDate(_eventsDbContext.Attendees, a => a.ModifiedDT ?? DateTime.MinValue);
        var cventAttendees = await _cventService.FetchData<CVENTAttendeeResponse>("Attendee", lastModifiedAttendees);

        // Map Cvent attendees to Cosmos entities
        var attendeeEntities = cventAttendees.Select(a => a.ToEntity()).ToList();
        await _eventsDbContext.Attendees.AddRangeAsync(attendeeEntities, cancellationToken);

        //Process Contact
        var lastModifiedContacts = GetLastModifiedDate(_eventsDbContext.Contacts, c => c.ModifiedDT ?? DateTime.MinValue);
        var cventContacts = await _cventService.FetchData<CVENTContactResponse>("Contact", lastModifiedContacts);

        // Map Cvent contacts to Cosmos entities
        var contactEntities = cventContacts.Select(c => c.ToEntity()).ToList();
        await _eventsDbContext.Contacts.AddRangeAsync(contactEntities, cancellationToken);
        await _eventsDbContext.SaveChangesAsync(cancellationToken);

        // Process SalesOrder
        // ???
        // Transaction and order used to create the sales order?
    }

    private DateTime GetLastModifiedDate<T>(DbSet<T> set, Func<T, DateTime> lastModifiedSelector)
    where T : class
    {
        // Get the most recent ModifiedDT; if none, fall back to default
        var last = set.AsNoTracking()
            .OrderBy(lastModifiedSelector)
            .Select(lastModifiedSelector)
            .FirstOrDefault();

        return last == default ? _defaultSyncDate : last;
    }


}
