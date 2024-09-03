using System;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Services;

namespace RoomBookingApp.Core.Processors
{
	public interface IRoomBookingRequestProcessor
	{
        RoomBokingResult BookRoom(RoomBookingRequest request);
    }
}

