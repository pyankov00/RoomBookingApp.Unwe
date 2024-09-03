using System;
using Microsoft.EntityFrameworkCore;
using RoomBookingApp.Core.Domain;
using RoomBookingApp.Persistence;
using RoomBookingApp.Persistence.Repositories;

namespace RoomBookingApp.Core.Tests
{
    public class RoomBookingServiceTest
    {
        [Fact]
        public void ShouldReturnAvailableRooms()
        {
            //Arrange
            var date = new DateTime(2021, 02, 02);

            var options = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
                .UseInMemoryDatabase("AvailableRoomTest")
                .Options;

            using var context = new RoomBookingAppDbContext(options);
            context.Add(new Room { Id = 1, Name = "Room 1", Price = 60, Currency = "BGN" });
            context.Add(new Room { Id = 2, Name = "Room 2", Price = 60, Currency = "BGN" });
            context.Add(new Room { Id = 3, Name = "Room 3", Price = 60, Currency = "BGN" });

            context.Add(new RoomBooking { RoomId = 1, Date = date, Email = "test@abv.bg", FullName = "TestTestov" });
            context.Add(new RoomBooking { RoomId = 2, Date = date.AddDays(-1), Email = "test@abv.bg", FullName = "TestTestov" });

            context.SaveChanges();


            var roomBookingService = new RoomBookingService(context);

            //Act
            var availableRooms = roomBookingService.GetAvailbaleRooms(date);

            //Assert
            Assert.Equal(2, availableRooms.Count());
            Assert.Contains(availableRooms, x => x.Id == 2);
            Assert.Contains(availableRooms, x => x.Id == 3);
            Assert.DoesNotContain(availableRooms, x => x.Id == 1);

        }

        [Fact]
        public void ShouldSaveRoomBooking()
        {
            var options = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
                .UseInMemoryDatabase("ShouldSaveTest")
                .Options;

            var roomBooking = new RoomBooking { RoomId = 1, Date = new DateTime(2021, 06, 09), Email = "test@abv.bg", FullName = "TestTestov" };

            using var context = new RoomBookingAppDbContext(options);
            var roomBookingService = new RoomBookingService(context);
            roomBookingService.Save(roomBooking);

            var bookings = context.RoomBookings.ToList();

            var booking = Assert.Single(bookings);
            Assert.Equal(roomBooking.Date, booking.Date);
            Assert.Equal(roomBooking.RoomId, booking.RoomId);
        }

    }
}

