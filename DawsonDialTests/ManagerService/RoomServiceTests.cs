//TEST DRIVEN DEVELOPMENT

//From Lucidchart:
// RoomService
// Attrributes:
//  _rooms : HashSet<IRoom>
// Methods:
//  + ReserveRoom(IRoom, DateTime, TimeSpan) : bool ---> Try reserving a room at a specific time range; returns success (bool)
//  + ReleaseRoom(IRoom, DateTime) : void ---> Frees the room
//  + GetRoomAvailability(IRoom) ---> Returns current availability (a bool?)

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Models.Enums;
using DawsonDial.Services.ManagerService;
using DawsonDial.Repositories.Interfaces;

namespace DawsonDialTests.ManagerService
{

    [TestClass]
    public class RoomServiceTests
    {
        //Declaring test class members, initialized to null and marked with '!' to 'silence' nullability warnings
        private RoomService _roomService = null!;
        private Mock<IRoomRepository> _mockRoomRepo = null!;
        private Mock<IEventRepository> _mockEventRepo = null!;

        [TestInitialize]
        public void Setup()
        {
            //Create a new mock repository for IRoomRepository using Moq
            _mockRoomRepo = new Mock<IRoomRepository>();
            _mockEventRepo = new Mock<IEventRepository>();
            _mockEventRepo.Setup(e => e.GetAllAsync()).ReturnsAsync(new List<Event>());

            //Pass the mocked repository into the RoomService constructor
            //This will allow us to isolate and test RoomService independently of actual DB logic!
            _roomService = new RoomService(_mockRoomRepo.Object, _mockEventRepo.Object);
        }

        /// <summary>
        /// Helper method to generate a mock Event object for testing purposes.
        /// </summary>
        /// <param name="title">Optional title for the mock event.</param>
        /// <returns>A mocked Event with default participants and a title.</returns>
        private Event CreateMockEvent(string title = "Test Event")
        {
            //Creates a mock 
            var mockEvent = new Mock<Event>();

            //Setup some mocked properties that will be accessed in tests later on
            mockEvent.Setup(e => e.Title).Returns(title);
            mockEvent.Setup(e => e.Participants).Returns(new HashSet<Person>());

            //Return the mock object as an actual Event instance
            return mockEvent.Object;
        }

        /// <summary>
        /// Helper function to generate a mock IEventRepository object for testing purposes.
        /// </summary>
        private Mock<IEventRepository> CreateMockEventRepo()
        {
            var mockRepo = new Mock<IEventRepository>();
            mockRepo.Setup(e => e.GetAllAsync()).ReturnsAsync(new List<Event>());
            return mockRepo;
        }

        /// <summary>
        /// Tests that AddRoomAsync adds a valid room by calling the repository's AddAsync method.
        /// </summary>
        [TestMethod]
        public async Task AddRoomAsync_ValidRoom_CallsRepositoryAdd()
        {
            //Arrange
            var room = new Office("3A.20", 2, false);

            //We setup the mock repository and service in the test class Setup method
            //So we only need to use _roomService and _mockRoomRepo here

            //Act
            await _roomService.AddRoomAsync(room);

            //Assert
            //Verify that the AddAsync method was called exactly once with the given room
            _mockRoomRepo.Verify(repo => repo.AddAsync(room), Times.Once);
        }

        /// <summary>
        /// Tests that AddRoomAsync throws ArgumentNullException when room is null.
        /// </summary>
        [TestMethod]
        public async Task AddRoomAsync_NullRoom_ShouldThrowArgumentNullException()
        {
            //Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);

            //Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                service.AddRoomAsync(null!)
            );
        }

        /// <summary>
        /// Tests that AddRoomAsync throws ArgumentException when the repository rejects duplicate room.
        /// </summary>
        [TestMethod]
        public async Task AddRoomAsync_RoomAlreadyExists_ShouldThrowArgumentException()
        {
            //Arrange
            var room = new Office("3A.20", 2, false);
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();

            //Here, simulating that adding a duplicate room causes an ArgumentException
            mockRepo
                .Setup(repo => repo.AddAsync(It.IsAny<Room>()))
                .ThrowsAsync(new ArgumentException("Room already exists"));

            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);

            //Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await service.AddRoomAsync(room);
            });
        }


        /// <summary>
        /// Tests that UpdateRoomAsync calls the repository with the updated room.
        /// </summary>
        [TestMethod]
        public async Task UpdateRoomAsync_ValidRoom_ShouldCallRepositoryUpdate()
        {
            //Arrange
            var room = new Office("3A.20", 2, false);
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);

            //Act
            await service.UpdateRoomAsync(room);

            //Assert
            mockRepo.Verify(r => r.UpdateAsync(room), Times.Once, "Expected repository's UpdateAsync to be called once with the room.");
        }

        /// <summary>
        /// Tests that UpdateRoomAsync throws ArgumentNullException when room is null.
        /// </summary>
        [TestMethod]
        public async Task UpdateRoomAsync_NullRoom_ShouldThrowArgumentNullException()
        {
            //Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);

            //Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await service.UpdateRoomAsync(null!);
            });
        }

        /// <summary>
        /// Tests that UpdateRoomAsync throws an ArgumentException when the room does not exist.
        /// </summary>
        [TestMethod]
        public async Task UpdateRoomAsync_RoomDoesNotExist_ShouldThrowArgumentException()
        {
            //Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var room = new Office("3A.20", 2, false);

            mockRepo.Setup(r => r.UpdateAsync(room)).ThrowsAsync(new ArgumentException("Room does not exist."));
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);

            //Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await service.UpdateRoomAsync(room);
            });
        }

        /// <summary>
        /// Tests that RemoveRoomAsync calls the repository to remove the room.
        /// </summary>
        [TestMethod]
        public async Task RemoveRoomAsync_ValidId_ShouldCallRepositoryDelete()
        {
            //Arrange
            var roomId = Guid.NewGuid();
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);

            //Act
            await service.RemoveRoomAsync(roomId);

            //Assert
            mockRepo.Verify(r => r.DeleteAsync(roomId), Times.Once, "Expected DeleteAsync to be called once.");
        }

        //test for removing a null room is not needed anymore, since RemoveRoomAsync takes a Guid — not a Room. So there's no null object passed

        /// <summary>
        /// Tests that RemoveRoomAsync throws ArgumentException if the room does not exist.
        /// </summary>
        [TestMethod]
        public async Task RemoveRoomAsync_RoomDoesNotExist_ShouldThrowArgumentException()
        {
            // Arrange
            var roomId = Guid.NewGuid();
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            mockRepo.Setup(r => r.DeleteAsync(roomId)).ThrowsAsync(new ArgumentException("Room not found."));
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await service.RemoveRoomAsync(roomId);
            });
        }

        /// <summary>
        /// Tests that ReserveRoom returns true when the room is available and reservation is valid.
        /// </summary>
        [TestMethod]
        public async Task ReserveRoom_ValidInputs_ShouldReturnTrue()
        {
            //Arrange
            var room = new Office("3A.20", 2, true);
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);
            var mockEvent = CreateMockEvent();

            //Act
            var result = await service.ReserveRoom(room, mockEvent, DateTime.Now.AddDays(1), TimeSpan.FromHours(1));

            //Assert
            Assert.IsTrue(result, "Reservation should succeed when the room is available.");
        }

        /// <summary>
        /// Tests that ReserveRoom returns false when the room is already booked at the specified time.
        /// </summary>
        [TestMethod]
        public async Task ReserveRoom_TimeSlotAlreadyBooked_ShouldReturnFalse()
        {
            //Arrange
            var room = new Office("3A.20", 2, true);
            var mockRepo = new Mock<IRoomRepository>();

            // Create a mock event that overlaps with desired time
            var existingEvent = new Mock<Event>();
            existingEvent.Setup(e => e.Title).Returns("Existing Event");
            existingEvent.Setup(e => e.Participants).Returns(new HashSet<Person>());
            existingEvent.SetupProperty(e => e.Room);
            existingEvent.SetupProperty(e => e.StartDateTime);
            existingEvent.SetupProperty(e => e.EndDateTime);
            existingEvent.SetupProperty(e => e.Status);

            // Setup the exact time we'll try to reserve
            var reservationDateTime = DateTime.Now.AddDays(1).AddMinutes(30);
            var reservationDuration = TimeSpan.FromHours(1);

            // Make sure the existing event overlaps with our desired time
            existingEvent.Object.Room = room;
            existingEvent.Object.StartDateTime = reservationDateTime.AddMinutes(-15);
            existingEvent.Object.EndDateTime = reservationDateTime.AddMinutes(45);
            existingEvent.Object.Status = EventStatus.Confirmed;

            var mockEventRepo = new Mock<IEventRepository>();
            mockEventRepo.Setup(e => e.GetAllAsync()).ReturnsAsync(new List<Event> { existingEvent.Object });

            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);

            // Create a new event to try to schedule
            var newEvent = new Mock<Event>();
            newEvent.Setup(e => e.Title).Returns("Test Event");
            newEvent.Setup(e => e.Participants).Returns(new HashSet<Person>());
            newEvent.SetupProperty(e => e.Room);
            newEvent.SetupProperty(e => e.StartDateTime);
            newEvent.SetupProperty(e => e.EndDateTime);

            //Act - try to reserve the already booked timeslot
            var result = await service.ReserveRoom(room, newEvent.Object, reservationDateTime, reservationDuration);

            //Assert
            Assert.IsFalse(result, "Reservation should fail when the time slot is already booked.");
        }

        /// <summary>
        /// Tests that ReserveRoom throws ArgumentNullException when room is null.
        /// </summary>
        [TestMethod]
        public async Task ReserveRoom_NullRoom_ThrowsArgumentNullException()
        {
            //Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);
            var mockEvent = CreateMockEvent();

            //Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await service.ReserveRoom(null!, mockEvent, DateTime.Now.AddDays(1), TimeSpan.FromHours(1));
            });
        }

        /// <summary>
        /// Tests that ReserveRoom throws ArgumentNullException when event is null.
        /// </summary>
        [TestMethod]
        public async Task ReserveRoom_NullEvent_ThrowsArgumentNullException()
        {
            //Arrange
            var room = new Office("3A.20", 2, true);
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);

            //Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await service.ReserveRoom(room, null!, DateTime.Now.AddDays(1), TimeSpan.FromHours(1));
            });
        }

        /// <summary>
        /// Tests that ReserveRoom throws ArgumentException when time span is zero.
        /// </summary>
        [TestMethod]
        public async Task ReserveRoom_ZeroTimeSpan_ThrowsArgumentException()
        {
            //Arrange
            var room = new Office("3A.20", 2, true);
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);
            var mockEvent = CreateMockEvent();

            //Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await service.ReserveRoom(room, mockEvent, DateTime.Now.AddDays(1), TimeSpan.Zero);
            });
        }

        /// <summary>
        /// Tests that ReserveRoom throws ArgumentException when time span is negative.
        /// </summary>
        [TestMethod]
        public async Task ReserveRoom_NegativeTimeSpan_ThrowsArgumentException()
        {
            //Arrange
            var room = new Office("3A.20", 2, true);
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);
            var mockEvent = CreateMockEvent();

            //Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await service.ReserveRoom(room, mockEvent, DateTime.Now.AddDays(1), TimeSpan.FromHours(-1));
            });
        }

        /// <summary>
        /// Tests that ReserveRoom throws ArgumentException when reservation time is in the past.
        /// </summary>
        [TestMethod]
        public async Task ReserveRoom_DateTimeInPast_ThrowsArgumentException()
        {
            //Arrange
            var room = new Office("3A.20", 2, true);
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);
            var mockEvent = CreateMockEvent();

            //Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await service.ReserveRoom(room, mockEvent, DateTime.Now.AddDays(-1), TimeSpan.FromHours(1));
            });
        }

        /// <summary>
        /// Tests that ReserveRoom allows reservation even if room hasn't been explicitly tracked.
        /// </summary>

        //In the updated RoomService, we removed the _rooms set, so there is no more explicit sort of room existence check. 
        //This means 'ReserveRoom_UntrackedRoom_ShouldStillReserve()' test is no longer valid or needed,
        //unless we manually enforce a "room must be registered first" rule (which we currently don't I believe).
        //So here's how we change the test to check reservation still works even if the room hasn't been added elsewhere:
        [TestMethod]
        public async Task ReserveRoom_UntrackedRoom_ShouldStillReserve()
        {
            //Arrange
            var room = new Office("3A.20", 2, true);
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);
            var mockEvent = CreateMockEvent();

            //Act
            var result = await service.ReserveRoom(room, mockEvent, DateTime.Now.AddDays(1), TimeSpan.FromHours(1));

            //Assert
            Assert.IsTrue(result, "Reservation should still succeed for untracked room.");
        }

        /// <summary>
        /// Tests that ReserveRoom throws ArgumentException when room does not have enough seats for event.
        /// </summary>
        [TestMethod]
        public async Task ReserveRoom_NotEnoughSeats_ThrowsArgumentException()
        {
            //Arrange
            var room = new Office("3A.20", 1, true); // 1 seat
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);

            var participants = new HashSet<Person>
            {
                new Mock<Person>().Object,
                new Mock<Person>().Object // 2 participants
            };

            var mockEvent = new Mock<Event>();
            mockEvent.Setup(e => e.Title).Returns("Test Event");
            mockEvent.Setup(e => e.Participants).Returns(participants);

            //Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await service.ReserveRoom(room, mockEvent.Object, DateTime.Now.AddDays(1), TimeSpan.FromHours(1));
            });
        }

        /// <summary>
        /// Tests that ReleaseRoom returns true when the room has a reservation.
        /// </summary>
        [TestMethod]
        public async Task ReleaseRoom_ShouldReturnTrue_WhenRoomHasReservation()
        {
            //Arrange
            var room = new Office("3A.20", 2, true);
            var mockRepo = new Mock<IRoomRepository>();

            // Create a mock event with proper setup
            var existingEvent = new Mock<Event>();
            existingEvent.Setup(e => e.Title).Returns("Test Event");
            existingEvent.Setup(e => e.Participants).Returns(new HashSet<Person>());
            existingEvent.SetupProperty(e => e.Room);
            existingEvent.SetupProperty(e => e.StartDateTime);
            existingEvent.SetupProperty(e => e.EndDateTime);
            existingEvent.SetupProperty(e => e.Status);

            // Set the release time we'll test with
            var releaseTime = DateTime.Now.AddDays(1).AddMinutes(30);

            // Make sure the event is properly set up
            existingEvent.Object.Room = room;
            existingEvent.Object.StartDateTime = releaseTime.AddMinutes(-15);
            existingEvent.Object.EndDateTime = releaseTime.AddMinutes(45);
            existingEvent.Object.Status = EventStatus.Confirmed;

            var mockEventRepo = new Mock<IEventRepository>();
            mockEventRepo.Setup(e => e.GetAllAsync()).ReturnsAsync(new List<Event> { existingEvent.Object });
            mockEventRepo.Setup(e => e.UpdateAsync(It.IsAny<Event>())).Returns(Task.CompletedTask);

            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);

            //Act
            bool result = await service.ReleaseRoom(room, releaseTime);

            //Assert
            Assert.IsTrue(result, "Release should succeed when the room has a reservation.");
            mockEventRepo.Verify(e => e.UpdateAsync(It.IsAny<Event>()), Times.Once);
        }

        /// <summary>
        /// Tests that ReleaseRoom returns false when the room has no reservation.
        /// </summary>
        [TestMethod]
        public async Task ReleaseRoom_ShouldReturnFalse_WhenRoomHasNoReservations()
        {
            //Arrange
            var room = new Office("3A.20", 2, true);
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);

            //Act
            bool result = await service.ReleaseRoom(room, DateTime.Now.AddDays(1));

            //Assert
            Assert.IsFalse(result, "Release should return false when the room has no reservation.");
        }

        /// <summary>
        /// Tests that ReleaseRoom returns false when the room has no reservation at the specific time.
        /// </summary>
        [TestMethod]
        public async Task ReleaseRoom_ShouldReturnFalse_WhenRoomHasNoReservationAtTime()
        {
            //Arrange
            var room = new Office("3A.20", 2, true);
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);
            var mockEvent = CreateMockEvent();

            var reservedTime = DateTime.Now.AddDays(1);
            await service.ReserveRoom(room, mockEvent, reservedTime, TimeSpan.FromMinutes(30));

            //Act
            bool result = await service.ReleaseRoom(room, DateTime.Now.AddDays(2)); // not overlapping

            //Assert
            Assert.IsFalse(result, "Release should return false when no reservation exists at the specified time.");
        }

        /// <summary>
        /// Tests that releasing a room makes it available again for reservation.
        /// </summary>
        [TestMethod]
        public async Task ReleaseRoom_ShouldActuallyMarkRoomAsAvailable()
        {
            //Arrange
            var room = new Office("3A.20", 2, true);
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);
            var mockEvent = CreateMockEvent();

            var reservationTime = DateTime.Now.AddDays(1);
            await service.ReserveRoom(room, mockEvent, reservationTime, TimeSpan.FromHours(1));

            //Act
            await service.ReleaseRoom(room, reservationTime.AddMinutes(30)); // releases during the reservation

            //Assert
            var isAvailable = await service.IsRoomAvailableAt(room, reservationTime, TimeSpan.FromHours(1));
            Assert.IsTrue(isAvailable, "Room should be available after being released.");
        }

        /// <summary>
        /// Tests that ReleaseRoom throws an ArgumentNullException when the room is null.
        /// </summary>
        [TestMethod]
        public async Task ReleaseRoom_NullRoom_ThrowsArgumentNullException()
        {
            //Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);

            //Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await service.ReleaseRoom(null!, DateTime.Now));
        }

        /// <summary>
        /// Tests that releasing a room with a date time in the past throws an ArgumentException.
        /// </summary>
        [TestMethod]
        public async Task ReleaseRoom_DateTimeInPast_ThrowsArgumentException()
        {
            //Arrange
            var room = new Office("3A.20", 2, false);
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);
            var mockEvent = CreateMockEvent();
            await service.ReserveRoom(room, mockEvent, DateTime.Now.AddDays(1), TimeSpan.FromHours(1));

            //Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await service.ReleaseRoom(room, DateTime.Now.AddDays(-1)));
        }

        /// <summary>
        /// Tests that releasing a room that has no reservations returns false.
        /// </summary>
        [TestMethod]
        public async Task ReleaseRoom_RoomDoesNotExist_ThrowsArgumentException()
        {
            //Arrange
            var room = new Office("3A.20", 2, false);
            var mockRepo = new Mock<IRoomRepository>();
            var mockEventRepo = CreateMockEventRepo();
            var service = new RoomService(mockRepo.Object, mockEventRepo.Object);

            //Act
            var result = await service.ReleaseRoom(room, DateTime.Now.AddDays(1));

            //Assert
            Assert.IsFalse(result, "Release should return false for a room that has no reservations.");
        }

        /// <summary>
        /// Tests that GetRoomByIdAsync returns the correct room.
        /// </summary>
        [TestMethod]
        public async Task GetRoomByIdAsync_ValidId_ReturnsRoom()
        {
            //Arrange
            var room = new Office("3A.20", 2, false);
            _mockRoomRepo.Setup(r => r.GetByIdAsync(room.RoomId)).ReturnsAsync(room);
            var service = new RoomService(_mockRoomRepo.Object, _mockEventRepo.Object);

            //Act
            var result = await service.GetRoomByIdAsync(room.RoomId);

            //Assert
            Assert.AreEqual(room, result);
            Assert.AreEqual(room.RoomId, result.RoomId);
        }

        /// <summary>
        /// Tests that GetRoomByIdAsync throws when room does not exist.
        /// </summary>
        [TestMethod]
        public async Task GetRoomByIdAsync_RoomDoesNotExist_ThrowsKeyNotFoundException()
        {
            //Arrange
            var id = Guid.NewGuid();
            _mockRoomRepo.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new KeyNotFoundException());
            var service = new RoomService(_mockRoomRepo.Object, _mockEventRepo.Object);

            //Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
                await service.GetRoomByIdAsync(id));
        }

        /// <summary>
        /// Tests that IsRoomCurrentlyAvailable returns true when the room is available and not reserved.
        /// </summary>
        [TestMethod]
        public async Task IsRoomCurrentlyAvailable_ShouldReturnTrue_WhenRoomIsAvailable()
        {
            //Arrange
            var room = new Office("3A.20", 2, true); // IsAvailable must be true
            var service = new RoomService(_mockRoomRepo.Object, _mockEventRepo.Object);

            //Act
            var available = await service.IsRoomCurrentlyAvailable(room);

            //Assert
            Assert.IsTrue(available, "Room should be reported as available when not reserved.");
        }

        /// <summary>
        /// Tests that IsRoomCurrentlyAvailable returns false when the room is marked unavailable.
        /// </summary>
        [TestMethod]
        public async Task IsRoomCurrentlyAvailable_ShouldReturnFalse_WhenRoomIsNotAvailable()
        {
            //Arrange
            var mockRoom = new Mock<Room>();
            mockRoom.Setup(r => r.IsAvailable).Returns(false);
            var roomService = new RoomService(_mockRoomRepo.Object, _mockEventRepo.Object);

            //Act
            var result = await roomService.IsRoomCurrentlyAvailable(mockRoom.Object);

            //Assert
            Assert.IsFalse(result, "Room should be reported as unavailable when IsAvailable is false.");
        }

        /// <summary>
        /// Tests that IsRoomCurrentlyAvailable throws ArgumentNullException when room is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task IsRoomCurrentlyAvailable_RoomIsNull_ThrowsArgumentNullException()
        {
            //Arrange
            var service = new RoomService(_mockRoomRepo.Object, _mockEventRepo.Object);

            //Act
            await service.IsRoomCurrentlyAvailable(null!);
        }

        /// <summary>
        /// Tests that IsRoomCurrentlyAvailable returns true when no reservations exist for a room.
        /// </summary>
        [TestMethod]
        public async Task IsRoomCurrentlyAvailable_UntrackedRoom_ShouldReturnTrue()
        {
            //Arrange
            var room = new Office("3A.20", 2, true); // room exists logically but is not reserved
            var service = new RoomService(_mockRoomRepo.Object, _mockEventRepo.Object);

            //Act
            var result = await service.IsRoomCurrentlyAvailable(room);

            //Assert
            Assert.IsTrue(result, "Untracked room should be considered available if no reservations exist.");
        }

        /// <summary>
        /// Tests that IsRoomAvailableAt returns true when the room is available at the given time.
        /// </summary>
        [TestMethod]
        public async Task IsRoomAvailableAt_ShouldReturnTrue_WhenRoomIsAvailable()
        {
            //Arrange
            var room = new Office("3A.20", 2, false);
            var futureTime = DateTime.Now.AddDays(1);
            var duration = TimeSpan.FromHours(1);

            //Act
            var available = await _roomService.IsRoomAvailableAt(room, futureTime, duration);

            //Assert
            Assert.IsTrue(available, "Room should be available when no conflicting reservations exist.");
        }

        /// <summary>
        /// Tests that IsRoomAvailableAt returns false when the room is already reserved at that time.
        /// </summary>
        [TestMethod]
        public async Task IsRoomAvailableAt_ShouldReturnFalse_WhenRoomIsNotAvailable()
        {
            //Arrange
            var room = new Office("3A.20", 2, true);

            // Create a mock event during the time we want to check
            var existingEvent = new Mock<Event>();
            existingEvent.Setup(e => e.Title).Returns("Existing Event");
            existingEvent.Setup(e => e.Participants).Returns(new HashSet<Person>());
            existingEvent.SetupProperty(e => e.Room);
            existingEvent.SetupProperty(e => e.StartDateTime);
            existingEvent.SetupProperty(e => e.EndDateTime);
            existingEvent.SetupProperty(e => e.Status);

            // Create a specific time to check availability
            var checkTime = DateTime.Now.AddDays(1).AddMinutes(30);
            var checkDuration = TimeSpan.FromHours(1);

            // Make sure the event overlaps with our time to check
            existingEvent.Object.Room = room;
            existingEvent.Object.StartDateTime = checkTime.AddMinutes(-15);
            existingEvent.Object.EndDateTime = checkTime.AddMinutes(45);
            existingEvent.Object.Status = EventStatus.Confirmed;

            // Setup mock repository to return the event
            var mockEventRepo = new Mock<IEventRepository>();
            mockEventRepo.Setup(e => e.GetAllAsync()).ReturnsAsync(new List<Event> { existingEvent.Object });

            var service = new RoomService(_mockRoomRepo.Object, mockEventRepo.Object);

            //Act - Check if the room is available during the overlapping time
            bool isAvailable = await service.IsRoomAvailableAt(room, checkTime, checkDuration);

            //Assert
            Assert.IsFalse(isAvailable, "Room should not be available when it has a conflicting reservation.");
        }

        /// <summary>
        /// Tests that IsRoomAvailableAt throws an ArgumentNullException when the room is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task IsRoomAvailableAt_NullRoom_ThrowsArgumentNullException()
        {
            //Act
            await _roomService.IsRoomAvailableAt(null!, DateTime.Now.AddDays(1), TimeSpan.FromHours(1));
        }

        /// <summary>
        /// Tests that IsRoomAvailableAt returns true for rooms with no reservation data.
        /// </summary>
        [TestMethod]
        public async Task IsRoomAvailableAt_UntrackedRoom_ShouldReturnTrue()
        {
            //Arrange
            var room = new Office("3A.21", 2, true); // Room not added via AddRoomAsync

            //Act
            var result = await _roomService.IsRoomAvailableAt(room, DateTime.Now.AddDays(1), TimeSpan.FromHours(1));

            //Assert
            Assert.IsTrue(result, "Room with no reservations should be considered available.");
        }

        /// <summary>
        /// Tests that IsRoomAvailableAt throws an ArgumentException when the dateTime is in the past.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task IsRoomAvailableAt_DateTimeIsInPast_ThrowsArgumentException()
        {
            //Arrange
            var room = new Office("3A.20", 2, false);
            var mockEventRepo = CreateMockEventRepo();
            _mockRoomRepo.Setup(r => r.AddAsync(It.IsAny<Room>())).Returns(Task.CompletedTask);
            var service = new RoomService(_mockRoomRepo.Object, mockEventRepo.Object);
            service.AddRoomAsync(room).Wait();

            //Act
            await service.IsRoomAvailableAt(room, DateTime.Now.AddDays(-1), TimeSpan.FromHours(1));
        }

        /// <summary>
        /// Tests that IsRoomAvailableAt throws an ArgumentException when the duration is zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task IsRoomAvailableAt_TimeSpanIsZero_ThrowsArgumentException()
        {
            //Arrange
            var room = new Office("3A.20", 2, false);
            var mockEventRepo = CreateMockEventRepo();
            _mockRoomRepo.Setup(r => r.AddAsync(It.IsAny<Room>())).Returns(Task.CompletedTask);
            var service = new RoomService(_mockRoomRepo.Object, mockEventRepo.Object);
            service.AddRoomAsync(room).Wait();

            //Act
            await service.IsRoomAvailableAt(room, DateTime.Now.AddDays(1), TimeSpan.Zero);
        }

        /// <summary>
        /// Tests that IsRoomAvailableAt throws an ArgumentException when the duration is negative.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task IsRoomAvailableAt_TimeSpanIsNegative_ThrowsArgumentException()
        {
            //Arrange
            var room = new Office("3A.20", 2, false);
            var mockEventRepo = CreateMockEventRepo();
            _mockRoomRepo.Setup(r => r.AddAsync(It.IsAny<Room>())).Returns(Task.CompletedTask);
            var service = new RoomService(_mockRoomRepo.Object, mockEventRepo.Object);
            service.AddRoomAsync(room).Wait();

            //Act
            await service.IsRoomAvailableAt(room, DateTime.Now.AddDays(1), TimeSpan.FromHours(-1));
        }
    }
}
