using Microsoft.EntityFrameworkCore;
using RoomBookingApp.Core.Domain;

namespace RoomBookingApp.Persistence
{
    public class RoomBookingAppDbContext : DbContext
	{
        public RoomBookingAppDbContext(DbContextOptions<RoomBookingAppDbContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<RoomBooking> RoomBookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, Name = "Room A" },
                new Room { Id = 2, Name = "Room B" },
                new Room { Id = 3, Name = "Room C" }
                );
        }
    }
}

