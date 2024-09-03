using RoomBookingApp.Core.Enums;
using RoomBookingApp.Domain;

namespace RoomBookingApp.Core.Models
{
    public class RoomBokingResult : RoomBookingBase
    {
        public BookingResultFlag Flag { get; set; }

        public int? RoomBookingId { get; set; }
    }
}