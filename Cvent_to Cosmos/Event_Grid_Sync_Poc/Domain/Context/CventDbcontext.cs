using Event_Grid_Sync_Poc.Domain.Context.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace Event_Grid_Sync_Poc.Domain.Context
{
    public class CventDBContext : DbContext
    {
        private readonly IConfiguration _configuration;
        //public readonly string? blobStorageConnection;
        //public readonly string? blobStorageContainer;

        public CventDBContext(DbContextOptions<CventDBContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;

        }

        public virtual DbSet<Event> Event { get; set; }
        //public virtual DbSet<CventTicketType> TicketType { get; set; }
        //public virtual DbSet<CventAttendee> Attendee { get; set; }
        //public virtual DbSet<CventOrder> Order { get; set; }
        //public virtual DbSet<CventTransaction> Receipt { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
