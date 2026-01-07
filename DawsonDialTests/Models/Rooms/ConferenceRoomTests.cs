// Test_ConferenceRoomConstructor_ShouldInitializeProperties: Properties set correctly
// Test_ConferenceRoomConstructor_InitializesEmptyEventsList: ScheduledEvents initialized
// Test_ScheduleEvent_ShouldAddToScheduledEvents: Adding event increases list count
// Test_ScheduleEvent_ShouldPreventTimeConflicts: No overlapping events allowed
using DawsonDial.Models.Rooms;
using DawsonDial.Models.Events;
using Moq;

namespace DawsonDialTests.Models.Rooms
{
    [TestClass]
    public class ConferenceRoomTests
    {
        //1: Test if the constructor initializes properties correctly
        [TestMethod]
        public void Test_ConferenceRoomConstructor_ShouldInitializeProperties()
        {
            //Arrange
            string roomNumber = "3B.15";
            int seats = 50;
            bool hasProjector = true;

            //Act
            var room = new ConferenceRoom(roomNumber, seats, hasProjector);

            //Assert
            Assert.AreEqual(roomNumber, room.RoomNumber);
            Assert.AreEqual(seats, room.NumberOfSeats);
            Assert.AreEqual(hasProjector, room.HasProjector);
            Assert.IsTrue(room.IsAvailable, "conference room should be available by default");
        }

        //2: Test if EventList is initialized as an empty list at first
        [TestMethod]
        public void Test_ConferenceRoomConstructor_InitializesEmptyEventsList()
        {
            //Arrange & Act
            var room = new ConferenceRoom("3B.15", 50, true);

            //Assert
            Assert.IsNotNull(room.ScheduledEvents, "ScheduledEvents should be initialized");
            Assert.AreEqual(0, room.ScheduledEvents.Count, "ScheduledEvents should start empty at first");
        }

        //3: Testing if an event is added and is the first one in the list
        [TestMethod]
        public void Test_ScheduleEvent_ShouldAddToScheduledEvents()
        {
            //Arrange
            var room = new ConferenceRoom("3B.15", 50, true);

            var mockEvent = new Mock<Event>();
            room.ScheduleEvent(mockEvent.Object);

            //Assert
            Assert.AreEqual(1, room.ScheduledEvents.Count, "Scheduling an event should increase the list count of events");
            Assert.AreSame(mockEvent.Object, room.ScheduledEvents[0], "The added event should match the input!");
        }

        //4: Testing that no overlapping events are allowed
        //   For this test ConferenceRoom - ScheduleEVent() was modified
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_ScheduleEvent_ShouldPreventTimeConflicts()
        {
            //Arrange
            var room = new ConferenceRoom("3B.15", 100, true);

            var mockEvent1 = new Mock<Event>();
            mockEvent1.Setup(e => e.StartDateTime).Returns(new DateTime(2025, 4, 10, 10, 0, 0));
            mockEvent1.Setup(e => e.EndDateTime).Returns(new DateTime(2025, 4, 10, 11, 0, 0));

            var mockEvent2 = new Mock<Event>();
            mockEvent2.Setup(e => e.StartDateTime).Returns(new DateTime(2025, 4, 10, 10, 30, 0));
            mockEvent2.Setup(e => e.EndDateTime).Returns(new DateTime(2025, 4, 10, 11, 30, 0));

            room.ScheduleEvent(mockEvent1.Object);

            //Act - should throw an exception because of time conflict
            room.ScheduleEvent(mockEvent2.Object);
        }
    }
}
