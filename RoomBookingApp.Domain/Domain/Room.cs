using System;
namespace RoomBookingApp.Core.Domain
{
	public class Room
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public int Price { get; set; }

		public string Currency { get; set; }

		public List<RoomBooking> RoomBookings { get; set; }
	}
}

