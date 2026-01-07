using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.Enums;
using DawsonDial.Services.ManagerService;
using DawsonDial.Repositories.Interfaces;
using Moq;
/*
 * FIXES & ADJUSTMENTS TO UNIT TESTS FOR EVENT-SERVICE-TESTS
 * --------------------------------------
 * 1️) Duration Constraint Fix:
 *     The OfficeMeeting constructor enforces duration between 15 minutes and 1 hour (not inclusive),
 *     which is why i updated all test cases to use DateTime.Now.AddMinutes(59) instead of AddHours(1+) to avoid:
 *       System.ArgumentException: Duration must be shorter than 1 hour.
 *
 *
 * 2) Improved Time Consistency:
 *     Declared variables like:
 *         DateTime start = DateTime.Now;
 *         DateTime end = start.AddMinutes(59);
 *     Which ensures consistent durations without relying on multiple DateTime.Now calls, which could introduce subtle bugs and does so
 */
namespace DawsonDialTests
{
    /// <summary>
    /// Tests for the EventService class.
    /// </summary>
    [TestClass]
    public class EventServiceTests
    {
        private static readonly DateTime Start = DateTime.Now;
        private static readonly DateTime End = Start.AddMinutes(59);
        private static readonly DateTime FutureStart = Start.AddHours(2);
        private static readonly DateTime FutureEnd = FutureStart.AddMinutes(59);

        /// <summary>
        /// Tests adding a new event.
        /// </summary>
        [TestMethod]
        public async Task AddNewEvent_EventIsAdded()
        {
            var mockRepo = new Mock<IEventRepository>();
            EventService eventService = new EventService(mockRepo.Object);
            Mock<Room> mockRoom = new Mock<Room>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            Event testEvent = new OfficeMeeting("Team Meeting", "Discuss project", mockRoom.Object, Start, End, false, EventStatus.Planned, mockTeacher.Object);

            await eventService.AddNewEvent(testEvent);

            mockRepo.Verify(r => r.AddAsync(It.Is<Event>(e => e.Title == "Team Meeting")), Times.Once);
        }

        /// <summary>
        /// Tests updating an event.
        /// </summary>
        [TestMethod]
        public async Task UpdateEvent_EventIsUpdated()
        {
            var mockRepo = new Mock<IEventRepository>();
            EventService eventService = new EventService(mockRepo.Object);
            Mock<Room> mockRoom = new Mock<Room>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            Event updatedEvent = new OfficeMeeting("Team Meeting Updated", "Discuss project updated", mockRoom.Object, Start, End, false, EventStatus.InProgress, mockTeacher.Object);

            await eventService.UpdateEvent(updatedEvent);

            mockRepo.Verify(r => r.UpdateAsync(It.Is<Event>(e => e.Title == "Team Meeting Updated")), Times.Once);
        }

        /// <summary>
        /// Tests changing the status of an event.
        /// </summary>
        [TestMethod]
        public async Task ChangeEventStatus_EventStatusIsChanged()
        {
            var mockRepo = new Mock<IEventRepository>();
            EventService eventService = new EventService(mockRepo.Object);
            Mock<Room> mockRoom = new Mock<Room>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            Event testEvent = new OfficeMeeting("Team Meeting", "Discuss project", mockRoom.Object, Start, End, false, EventStatus.Planned, mockTeacher.Object);

            await eventService.ChangeEventStatus(testEvent, EventStatus.InProgress);

            Assert.AreEqual(EventStatus.InProgress, testEvent.Status);
            mockRepo.Verify(r => r.UpdateAsync(testEvent), Times.Once);
        }

        /// <summary>
        /// Tests removing an event.
        /// </summary>
        [TestMethod]
        public async Task RemoveEvent_EventIsRemoved()
        {
            var mockRepo = new Mock<IEventRepository>();
            EventService eventService = new EventService(mockRepo.Object);
            Mock<Room> mockRoom = new Mock<Room>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            Event testEvent = new OfficeMeeting("Team Meeting", "Discuss project", mockRoom.Object, Start, End, false, EventStatus.Planned, mockTeacher.Object);

            await eventService.RemoveEvent(testEvent);

            mockRepo.Verify(r => r.DeleteAsync(testEvent.EventId), Times.Once);
        }

        /// <summary>
        /// Tests adding a participant to an event.
        /// </summary>
        [TestMethod]
        public async Task AddParticipantToEvent_ParticipantIsAdded()
        {
            var mockRepo = new Mock<IEventRepository>();
            EventService eventService = new EventService(mockRepo.Object);

            // Set NumberOfSeats on the mock room because MaxCapacity depends on it.
            // Without this, the event is considered full by default and throws an exception.
            Mock<Room> mockRoom = new Mock<Room>();
            mockRoom.Setup(r => r.NumberOfSeats).Returns(10);

            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Student> mockStudent = new Mock<Student>();

            Event testEvent = new OfficeMeeting("Team Meeting", "Discuss project", mockRoom.Object, Start, End, false, EventStatus.Planned, mockTeacher.Object);

            await eventService.AddParticipantToEvent(testEvent, mockStudent.Object);

            Assert.AreEqual(1, testEvent.Participants.Count());
            Assert.IsTrue(testEvent.Participants.Contains(mockStudent.Object));
            mockRepo.Verify(r => r.UpdateAsync(testEvent), Times.Once);
        }

        /// <summary>
        /// Tests removing a participant from an event.
        /// </summary>
        [TestMethod]
        public async Task RemoveParticipantFromEvent_ParticipantIsRemoved()
        {
            var mockRepo = new Mock<IEventRepository>();
            EventService eventService = new EventService(mockRepo.Object);

            // Set NumberOfSeats on the mock room because MaxCapacity depends on it.
            // Without this, the event is considered full by default and throws an exception.
            Mock<Room> mockRoom = new Mock<Room>();
            mockRoom.Setup(r => r.NumberOfSeats).Returns(10);

            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Student> mockStudent = new Mock<Student>();

            Event testEvent = new OfficeMeeting("Team Meeting", "Discuss project", mockRoom.Object, Start, End, false, EventStatus.Planned, mockTeacher.Object);
            testEvent.AddParticipant(mockStudent.Object);

            await eventService.RemoveParticipantFromEvent(testEvent, mockStudent.Object);

            Assert.AreEqual(0, testEvent.Participants.Count());
            Assert.IsFalse(testEvent.Participants.Contains(mockStudent.Object));
            mockRepo.Verify(r => r.UpdateAsync(testEvent), Times.Once);
        }

        /// <summary>
        /// Tests checking if an event has a participant.
        /// </summary>
        [TestMethod]
        public async Task CheckEventHasParticipant_EventHasParticipant()
        {
            var mockRepo = new Mock<IEventRepository>();
            EventService eventService = new EventService(mockRepo.Object);

            // Set NumberOfSeats on the mock room because MaxCapacity depends on it.
            // Without this, the event is considered full by default and throws an exception.
            Mock<Room> mockRoom = new Mock<Room>();
            mockRoom.Setup(r => r.NumberOfSeats).Returns(10);

            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Student> mockStudent = new Mock<Student>();

            Event testEvent = new OfficeMeeting("Team Meeting", "Discuss project", mockRoom.Object, Start, End, false, EventStatus.Planned, mockTeacher.Object);
            testEvent.AddParticipant(mockStudent.Object);

            mockRepo.Setup(r => r.GetByIdAsync(testEvent.EventId)).ReturnsAsync(testEvent);
            bool result = await eventService.CheckEventHasParticipant(testEvent, mockStudent.Object);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Tests removing a participant from an event.
        /// </summary>
        [TestMethod]
        public async Task CheckEventHasParticipant_EventDoesNotHaveParticipant()
        {
            var mockRepo = new Mock<IEventRepository>();
            EventService eventService = new EventService(mockRepo.Object);
            Mock<Room> mockRoom = new Mock<Room>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Student> mockStudent = new Mock<Student>();

            Event testEvent = new OfficeMeeting("Team Meeting", "Discuss project", mockRoom.Object, Start, End, false, EventStatus.Planned, mockTeacher.Object);

            mockRepo.Setup(r => r.GetByIdAsync(testEvent.EventId)).ReturnsAsync(testEvent);

            bool result = await eventService.CheckEventHasParticipant(testEvent, mockStudent.Object);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests checking for schedule conflicts between two events when they conflict.
        /// </summary>
        [TestMethod]
        public void CheckScheduleConflicts_EventsConflict()
        {
            var mockRepo = new Mock<IEventRepository>();
            EventService eventService = new EventService(mockRepo.Object);
            Mock<Room> mockRoom = new Mock<Room>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            Event testEvent1 = new OfficeMeeting("Team Meeting", "Discuss project", mockRoom.Object, Start, End, false, EventStatus.Planned, mockTeacher.Object);
            Event testEvent2 = new OfficeMeeting("Team Meeting", "Discuss project", mockRoom.Object, Start, End, false, EventStatus.Planned, mockTeacher.Object);

            bool result = eventService.CheckScheduleConflicts(testEvent1, testEvent2);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Tests checking for schedule conflicts between two events when they do not conflict.
        /// </summary>
        [TestMethod]
        public void CheckScheduleConflicts_EventsDoNotConflict()
        {
            var mockRepo = new Mock<IEventRepository>();
            EventService eventService = new EventService(mockRepo.Object);
            Mock<Room> mockRoom = new Mock<Room>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            Event testEvent1 = new OfficeMeeting("Team Meeting", "Discuss project", mockRoom.Object, Start, End, false, EventStatus.Planned, mockTeacher.Object);
            Event testEvent2 = new OfficeMeeting("Team Meeting", "Discuss project", mockRoom.Object, FutureStart, FutureEnd, false, EventStatus.Planned, mockTeacher.Object);

            bool result = eventService.CheckScheduleConflicts(testEvent1, testEvent2);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests getting all events from the event service.
        /// </summary>
        [TestMethod]
        public async Task GetAllEvents_ReturnsAllEvents()
        {
            var mockRepo = new Mock<IEventRepository>();
            EventService eventService = new EventService(mockRepo.Object);
            Mock<Room> mockRoom = new Mock<Room>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            Event testEvent1 = new OfficeMeeting("Team Meeting", "Discuss project", mockRoom.Object, Start, End, false, EventStatus.Planned, mockTeacher.Object);
            Event testEvent2 = new OfficeMeeting("Team Meeting", "Discuss project", mockRoom.Object, FutureStart, FutureEnd, false, EventStatus.Planned, mockTeacher.Object);

            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Event> { testEvent1, testEvent2 });

            var events = await eventService.GetAllEvents();

            Assert.AreEqual(2, events.Count());
            Assert.IsTrue(events.Contains(testEvent1));
            Assert.IsTrue(events.Contains(testEvent2));
        }
    }
}
