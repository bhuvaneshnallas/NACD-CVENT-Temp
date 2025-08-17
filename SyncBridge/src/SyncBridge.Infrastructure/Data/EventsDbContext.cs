using Microsoft.EntityFrameworkCore;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models.CVENT;

namespace SyncBridge.Infrastructure.Data;

public class EventsDbContext : DbContext, IEventsDbContext
{
    public EventsDbContext(DbContextOptions<EventsDbContext> options)
        : base(options) { }

    public virtual DbSet<EventEntity> Events { get; set; }
    public virtual DbSet<TicketType> TicketTypes { get; set; }
    public virtual DbSet<Attendee> Attendees { get; set; }
    public virtual DbSet<Contact> Contacts { get; set; }
    public virtual DbSet<SalesOrder> SalesOrders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventEntity>().ToContainer("Events").HasPartitionKey(e => e.id).OwnsMany(e => e.Planners);
        modelBuilder.Entity<TicketType>().ToContainer("TicketTypes").HasPartitionKey(e => e.id);
        modelBuilder.Entity<Attendee>().ToContainer("Attendees").HasPartitionKey(e => e.id);
        modelBuilder.Entity<Contact>().ToContainer("Contacts").HasPartitionKey(e => e.id);
        modelBuilder.Entity<SalesOrder>().ToContainer("SalesOrders").HasPartitionKey(e => e.id);
    }
}
