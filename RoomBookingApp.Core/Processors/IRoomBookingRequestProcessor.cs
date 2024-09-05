using System;
using RoomBookingApp.Core.Domain;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Services;

namespace RoomBookingApp.Core.Processors
{
	public interface IRoomBookingRequestProcessor
	{
        RoomBokingResult BookRoom(RoomBookingRequest request);

        IEnumerable<Room> GetAvailableRooms(DateTime date);

        IEnumerable<RoomBooking> GetRoomBookings(DateTime date);
    }
}

