using Microsoft.AspNetCore.Mvc;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;

namespace RoomBookingApp.Api.Controllers
{
    [ApiController]
	[Route("[controller]")]
	public class RoomBookingController : ControllerBase
	{
		private readonly IRoomBookingRequestProcessor _roomBookingProcessor;

        public RoomBookingController(IRoomBookingRequestProcessor roomBookingProcessor)
        {
            _roomBookingProcessor = roomBookingProcessor;
        }

        [HttpPost("/book")]
        public IActionResult BookRoom(RoomBookingRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = _roomBookingProcessor.BookRoom(request);
                if (result.Flag == Core.Enums.BookingResultFlag.Success)
                {
                    return Ok(result);
                }

                ModelState.AddModelError(nameof(RoomBookingRequest.Date), "No Rooms Available For Given Date");
            }

            return BadRequest(ModelState);
        }

        [HttpGet("/rooms")]
        public IActionResult GetAvailableRooms([FromQuery]DateTime date)
        {
            if (date.Date < DateTime.Now.Date)
            {
                return BadRequest("Date Must be In The Future");
            }

            return Ok(_roomBookingProcessor.GetAvailableRooms(date));
        }

        [HttpGet("/roomBookings")]
        public IActionResult GetRoomBookings([FromQuery] DateTime date)
        {
            return Ok(_roomBookingProcessor.GetRoomBookings(date));
        }
    }
}

