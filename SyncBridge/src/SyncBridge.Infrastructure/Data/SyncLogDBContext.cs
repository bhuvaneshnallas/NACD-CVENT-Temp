using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SyncBridge.Domain.Models;

namespace SyncBridge.Infrastructure.Data;

public class SyncLogDBContext : DbContext
{
    public SyncLogDBContext(DbContextOptions<SyncLogDBContext> options)
        : base(options) { }

    public virtual DbSet<SyncLog> SyncLog { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<SyncLog>().ToContainer("Synclog").HasPartitionKey(e => e.id);
    }
}
