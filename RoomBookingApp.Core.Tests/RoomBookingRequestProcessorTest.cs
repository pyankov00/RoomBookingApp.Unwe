using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using RoomBookingApp.Core.Services;
using Shouldly;
using Moq;
using RoomBookingApp.Core.Domain;
using RoomBookingApp.Core.Enums;

namespace RoomBookingApp.Core.Tests
{
    public class RoomBookingRequestProcessorTest
	{
        private RoomBookingRequestProcessor _processor;
        private RoomBookingRequest _request;
        private Mock<IRoomBookingService> _roomBookingServiceMock;
        private List<Room> _availableRooms;
        private List<RoomBooking> _roomBookings;

        public RoomBookingRequestProcessorTest()
        {
            _request = new RoomBookingRequest
            {
                FullName = "Test Name",
                Email = "test@request.com",
                Date = new DateTime(2023, 1, 1)
            };

            _availableRooms = new List<Room>() { new Room() { Id = 1 }, new Room() { Id = 2 } };
            _roomBookings = new List<RoomBooking>() { new RoomBooking { RoomId = _availableRooms[1].Id, Date = _request.Date } };
            _roomBookingServiceMock = new Mock<IRoomBookingService>();
            _roomBookingServiceMock.Setup(q => q.GetAvailbaleRooms(_request.Date))
                .Returns(_availableRooms);
            _roomBookingServiceMock.Setup(q => q.GetRoomBookings(_request.Date))
                .Returns(_roomBookings);

            _processor = new RoomBookingRequestProcessor(_roomBookingServiceMock.Object);
        }

		[Fact]
		public void ShouldReturnRoomBookingResponseWithRequestValues()
		{
			//Act
			RoomBokingResult result = _processor.BookRoom(_request);

			//Assert
			Assert.NotNull(result);
			Assert.Equal(_request.FullName, result.FullName);
			Assert.Equal(_request.Email, result.Email);
			Assert.Equal(_request.Date, result.Date);
        }

		[Fact]
		public void ShouldThrowExceptionForNullRequest()
		{
			var exception = Assert.Throws<ArgumentNullException>(() => _processor.BookRoom(null));
			exception.ParamName.ShouldBe("request");
		}

        [Fact]
        public void ShouldSaveRoomBookingRequest()
        {
            RoomBooking savedBooking = null;
            _roomBookingServiceMock.Setup(q => q.Save(It.IsAny<RoomBooking>()))
                .Callback<RoomBooking>(booking =>
                {
                    savedBooking = booking;
                });

            _processor.BookRoom(_request);

            _roomBookingServiceMock.Verify(q => q.Save(It.IsAny<RoomBooking>()), Times.Once);

            savedBooking.ShouldNotBeNull();
            savedBooking.FullName.ShouldBe(_request.FullName);
            savedBooking.Date.ShouldBe(_request.Date);
            savedBooking.Email.ShouldBe(_request.Email);
            savedBooking.RoomId.ShouldBe(_availableRooms.First().Id);
        }

        [Fact]
        public void ShouldNotSaveRoomBookingRequestIfNoneAvailable()
        {
            _availableRooms.Clear();
            _processor.BookRoom(_request);

            _roomBookingServiceMock.Verify(q => q.Save(It.IsAny<RoomBooking>()), Times.Never);
        }

        [Theory]
        [InlineData(BookingResultFlag.Failure, false)]
        [InlineData(BookingResultFlag.Success, true)]
        public void ShouldReturnSuccessOrFailureFlagInResult(BookingResultFlag bookingSuccessFlag, bool isAvailable)
        {
            if (!isAvailable)
            {
                _availableRooms.Clear();
            }

            var result = _processor.BookRoom(_request);
            bookingSuccessFlag.ShouldBe(result.Flag);
        }


        [Theory]
        [InlineData(1, true)]
        [InlineData(null, false)]
        public void ShouldReturnRoomBookingIdInResult(int? roomBookingId, bool isAvailable)
        {
            if (!isAvailable)
            {
                _availableRooms.Clear();
            }
            else
            {
                _roomBookingServiceMock.Setup(q => q.Save(It.IsAny<RoomBooking>()))
                .Callback<RoomBooking>(booking =>
                {
                    booking.Id = roomBookingId.Value;
                });
            }

            var result = _processor.BookRoom(_request);
            result.RoomBookingId.ShouldBe(roomBookingId);
        }

        [Fact]
        public void ShouldReturnAvailableRooms()
        {
            var result = _processor.GetAvailableRooms(_request.Date).ToList();
            Assert.Equal(_availableRooms, result);
        }

        [Fact]
        public void ShouldReturnRoomBookings()
        {
            var result = _processor.GetRoomBookings(_request.Date).ToList();
            Assert.Equal(_roomBookings, result);
        }
    }
    
}

