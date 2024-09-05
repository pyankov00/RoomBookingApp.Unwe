using RoomBookingApp.Core.Domain;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Services;
using RoomBookingApp.Domain;

namespace RoomBookingApp.Core.Processors
{
    public class RoomBookingRequestProcessor : IRoomBookingRequestProcessor
    {
        private readonly IRoomBookingService _roomBookingService;

        public RoomBookingRequestProcessor(IRoomBookingService roomBookingService)
        {
            _roomBookingService = roomBookingService;
        }


        public RoomBokingResult BookRoom(RoomBookingRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var availableRooms = _roomBookingService.GetAvailbaleRooms(request.Date);
            var result = CreateRoomBookingObject<RoomBokingResult>(request); 

            if (availableRooms.Any())
            {
                var room = availableRooms.First();
                var roomBooking = CreateRoomBookingObject<RoomBooking>(request);
                roomBooking.RoomId = room.Id;

               _roomBookingService.Save(roomBooking);

                result.Flag = BookingResultFlag.Success;
                result.RoomBookingId = roomBooking.Id;
            }
            else
            {
                result.Flag = BookingResultFlag.Failure;
            }



            return result;
        }

        public IEnumerable<Room> GetAvailableRooms(DateTime date)
         => _roomBookingService.GetAvailbaleRooms(date);

        public IEnumerable<RoomBooking> GetRoomBookings(DateTime date)
         => _roomBookingService.GetRoomBookings(date);

        private TRoomBooking CreateRoomBookingObject<TRoomBooking>(RoomBookingRequest roomBookingRequest)
            where TRoomBooking : RoomBookingBase, new()
        {
            return new TRoomBooking
            {
                FullName = roomBookingRequest.FullName,
                Date = roomBookingRequest.Date,
                Email = roomBookingRequest.Email
            };
        }
    }
}