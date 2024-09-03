using System;
using RoomBookingApp.Core.Domain;
using RoomBookingApp.Core.Services;

namespace RoomBookingApp.Persistence.Repositories
{
	public class RoomBookingService : IRoomBookingService
	{
        private readonly RoomBookingAppDbContext _context;

        public RoomBookingService(RoomBookingAppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Room> GetAvailbaleRooms(DateTime date)
        {
            var availableRooms = _context.Rooms.Where(x => !x.RoomBookings.Any(y => y.Date == date)).ToList();

            return availableRooms;
        }

        public void Save(RoomBooking roomBooking)
        {
            _context.Add(roomBooking);
            _context.SaveChanges();
        }
    }
}

