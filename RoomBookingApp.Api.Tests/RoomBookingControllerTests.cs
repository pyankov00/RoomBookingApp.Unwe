using Microsoft.AspNetCore.Mvc;
using Moq;
using RoomBookingApp.Api.Controllers;
using RoomBookingApp.Core.Domain;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using Shouldly;

namespace RoomBookingApp.Api.Tests
{
    public class RoomBookingControllerTests
    {
        private readonly Mock<IRoomBookingRequestProcessor> _roomBookingProcessor;
        private readonly RoomBookingController _controller;
        private readonly RoomBookingRequest _request;
        private readonly RoomBokingResult _result;
        private readonly List<RoomBooking> _roomBookings = new List<RoomBooking>();

        public RoomBookingControllerTests()
        {
            _roomBookingProcessor = new Mock<IRoomBookingRequestProcessor>();
            _controller = new RoomBookingController(_roomBookingProcessor.Object);
            _request = new RoomBookingRequest();
            _result = new RoomBokingResult();
            _roomBookings.Add(new RoomBooking { RoomId = 1, Date = new DateTime(2021, 06, 09), Email = "test@abv.bg", FullName = "TestTestov" });

            _roomBookingProcessor.Setup(x => x.BookRoom(_request)).Returns(_result);
            _roomBookingProcessor.Setup(x => x.GetRoomBookings(DateTime.UtcNow)).Returns(_roomBookings);
        }

        [Theory]
        [InlineData(1, true, typeof(OkObjectResult), BookingResultFlag.Success)]
        [InlineData(0, false, typeof(BadRequestObjectResult), BookingResultFlag.Failure)]
        public void ShouldCallBookingMethodWhenValid(int expectedMethodCalls, bool isModelValid,
            Type expectedActionResultType, BookingResultFlag bookingResultFlag)
        {
            // Arrange
            if (!isModelValid)
            {
                _controller.ModelState.AddModelError("Key", "ErrorMessage");
            }

            _result.Flag = bookingResultFlag;


            // Act
            var result = _controller.BookRoom(_request);

            // Assert
            result.ShouldBeOfType(expectedActionResultType);
            _roomBookingProcessor.Verify(x => x.BookRoom(_request), Times.Exactly(expectedMethodCalls));

        }

        [Fact]
        public void GetAvailableRoomsDateInThePastReturnsBadRequest()
        {
            // Arrange
            DateTime pastDate = DateTime.Now.AddDays(-1);

            // Act
            var result = _controller.GetAvailableRooms(pastDate);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Date Must be In The Future", badRequestResult.Value);
        }

        [Fact]
        public void GetAvailableRoomsTodaysDateReturnsOkWithAvailableRooms()
        {
            // Arrange
            DateTime todayDate = DateTime.Now.Date;
            var availableRooms = new List<Room>(); // Assuming Room is a model class
            _roomBookingProcessor.Setup(x => x.GetAvailableRooms(todayDate)).Returns(availableRooms);

            // Act
            var result = _controller.GetAvailableRooms(todayDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(availableRooms, okResult.Value);
        }

        [Fact]
        public void GetAvailableRoomsDateInTheFutureReturnsOkWithAvailableRooms()
        {
            // Arrange
            DateTime futureDate = DateTime.Now.AddDays(1);
            var availableRooms = new List<Room>(); // Assuming Room is a model class
            _roomBookingProcessor.Setup(x => x.GetAvailableRooms(futureDate)).Returns(availableRooms);

            // Act
            var result = _controller.GetAvailableRooms(futureDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(availableRooms, okResult.Value);
        }

        [Fact]
        public void GetRoomBookingsValidDateReturnsOkWithRoomBookings()
        {
            // Arrange
            DateTime validDate = DateTime.Now;

            // Act
            var result = _controller.GetRoomBookings(validDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(_roomBookings, okResult.Value);
            _roomBookingProcessor.Verify(x => x.GetRoomBookings(validDate), Times.Once);
        }
    }
}