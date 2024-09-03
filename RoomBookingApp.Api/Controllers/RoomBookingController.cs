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

        [HttpPost]
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
    }
}

