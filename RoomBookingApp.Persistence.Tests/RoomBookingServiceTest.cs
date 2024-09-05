using System;
using System.ComponentModel.DataAnnotations;
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

        [Fact]
        public void ShouldReturnRoomBookings()
        {
            //Arrange
            var date = new DateTime(2021, 02, 02);

            var options = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
                .UseInMemoryDatabase("RoomBookingTests")
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
            var roomBookings = roomBookingService.GetRoomBookings(date);

            //Assert
            Assert.Single(roomBookings);
            Assert.Contains(roomBookings, x => x.RoomId == 1);
            Assert.DoesNotContain(roomBookings, x => x.RoomId == 3);
            Assert.DoesNotContain(roomBookings, x => x.RoomId == 2);

        }

        [Fact]
        public void Validate_ShouldReturnError_WhenDateIsInThePast()
        {
            // Arrange
            var model = new RoomBooking { Date = DateTime.Now.Date.AddDays(-1) };
            var context = new ValidationContext(model);

            // Act
            var results = model.Validate(context);

            // Assert
            Assert.Single(results);
            Assert.Equal("Date Must be In The Future", results.First().ErrorMessage);
            Assert.Contains(nameof(RoomBooking.Date), results.First().MemberNames);
        }

        [Fact]
        public void Validate_ShouldReturnError_WhenDateIsToday()
        {
            // Arrange
            var model = new RoomBooking { Date = DateTime.Now.Date };
            var context = new ValidationContext(model);

            // Act
            var results = model.Validate(context);

            // Assert
            Assert.Single(results);
            Assert.Equal("Date Must be In The Future", results.First().ErrorMessage);
            Assert.Contains(nameof(RoomBooking.Date), results.First().MemberNames);
        }

        [Fact]
        public void Validate_ShouldNotReturnError_WhenDateIsInTheFuture()
        {
            // Arrange
            var model = new RoomBooking { Date = DateTime.Now.Date.AddDays(1) };
            var context = new ValidationContext(model);

            // Act
            var results = model.Validate(context);

            // Assert
            Assert.Empty(results);
        }

    }
}

