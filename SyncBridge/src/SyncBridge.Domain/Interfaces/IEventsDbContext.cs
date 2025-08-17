using Microsoft.EntityFrameworkCore;
using SyncBridge.Domain.Models.CVENT;

namespace SyncBridge.Domain.Interfaces;

public interface IEventsDbContext
{
    DbSet<EventEntity> Events { get; set; }
    DbSet<TicketType> TicketTypes { get; set; }
    DbSet<Attendee> Attendees { get; set; }
    DbSet<Contact> Contacts { get; set; }
    DbSet<SalesOrder> SalesOrders { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
