using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.Enums;
using Moq;

namespace DawsonDialTests
{
    /// <summary>
    /// Tests the Conference class.
    /// </summary>
    [TestClass]
    public class ConferenceTests
    {
        /// <summary>
        /// Tests that the constructor initializes properties correctly.
        /// </summary>
        [TestMethod]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            string title = "Test Conference";
            string description = "Test Description";
            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.Now.AddHours(2);
            bool isRecurring = false;
            EventStatus status = EventStatus.Planned;

            Mock<ConferenceRoom> mockRoom = new Mock<ConferenceRoom>();
            Mock<Person> mockSpeaker = new Mock<Person>();

            // Act
            Conference conference = new Conference(
                title,
                description,
                mockRoom.Object,
                startTime,
                endTime,
                isRecurring,
                status,
                mockSpeaker.Object
            );

            // Assert
            Assert.AreEqual(title, conference.Title, "Title not initialized correctly.");
            Assert.AreEqual(description, conference.Description, "Description not initialized correctly.");
            Assert.AreEqual(mockRoom.Object, conference.Room, "Room not initialized correctly.");
            Assert.AreEqual(startTime, conference.StartDateTime, "Start time not initialized correctly.");
            Assert.AreEqual(endTime, conference.EndDateTime, "End time not initialized correctly.");
            Assert.AreEqual(isRecurring, conference.IsRecurring, "IsRecurring not initialized correctly.");
            Assert.AreEqual(status, conference.Status, "Status not initialized correctly.");
            Assert.AreEqual(mockSpeaker.Object, conference.Speaker, "Speaker not initialized correctly.");
            Assert.IsNotNull(conference.Participants, "Participants list is null.");
            Assert.AreEqual(0, conference.Participants.Count, "Participants list should be empty.");
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException when the speaker is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullSpeaker_ShouldThrowArgumentNullException()
        {
            // Arrange
            Mock<ConferenceRoom> mockConferenceRoom = new Mock<ConferenceRoom>();

            // Act
            Conference conference = new Conference(
                "Test Title",
                "Test description",
                mockConferenceRoom.Object,
                DateTime.Now,
                DateTime.Now.AddHours(1),
                false,
                EventStatus.Planned,
                null!);
        }

        /// <summary>
        /// Tests that RegisterAttendee adds attendee to the participants list.
        /// </summary>
        [TestMethod]
        public void RegisterAttendee_ShouldUpdateParticipantsList()
        {
            // Arrange
            Mock<ConferenceRoom> mockConferenceRoom = new Mock<ConferenceRoom>();
            mockConferenceRoom.Setup(room => room.NumberOfSeats).Returns(30);
            Mock<Person> mockSpeaker = new Mock<Person>();
            Mock<Person> mockAttendee = new Mock<Person>();

            Conference conference = new Conference(
                "Test Title",
                "Test description",
                mockConferenceRoom.Object,
                DateTime.Now,
                DateTime.Now.AddHours(1),
                false,
                EventStatus.Planned,
                mockSpeaker.Object
                );

            // Act
            conference.RegisterAttendee(mockAttendee.Object);

            // Assert
            Assert.AreEqual(1, conference.Participants.Count,
                            "Participants should have one attendee.");
            Assert.IsTrue(conference.Participants.Contains(mockAttendee.Object),
                            "Attendee should be in Participants.");
        }

        /// <summary>
        /// Tests that UnregisterAttendee removes an attendee from the Participants list.
        /// </summary>
        [TestMethod]
        public void UnregisterAttendee_ShouldUpdateParticipantsList()
        {
            // Arrange
            Mock<ConferenceRoom> mockConferenceRoom = new Mock<ConferenceRoom>();
            mockConferenceRoom.Setup(room => room.NumberOfSeats).Returns(30);
            Mock<Person> mockSpeaker = new Mock<Person>();
            Mock<Person> mockAttendee = new Mock<Person>();

            Conference conference = new Conference(
                "Test Title",
                "Test description",
                mockConferenceRoom.Object,
                DateTime.Now,
                DateTime.Now.AddHours(1),
                false,
                EventStatus.Planned,
                mockSpeaker.Object
                );

            conference.RegisterAttendee(mockAttendee.Object);

            // Act
            conference.UnregisterAttendee(mockAttendee.Object);

            // Assert
            Assert.AreEqual(0, conference.Participants.Count,
                            "Participants should be empty.");
            Assert.IsFalse(conference.Participants.Contains(mockAttendee.Object),
                            "Attendee should not be in Participants.");
        }

        /// <summary>
        /// Tests if IsAttendeeRegistered returns true when the attendee is registered.
        /// </summary>
        [TestMethod]
        public void IsAttendeeRegistered_ReturnsTrue()
        {
            // Arrange
            Mock<ConferenceRoom> mockConferenceRoom = new Mock<ConferenceRoom>();
            mockConferenceRoom.Setup(room => room.NumberOfSeats).Returns(10);
            Mock<Person> mockSpeaker = new Mock<Person>();
            Mock<Person> mockAttendee = new Mock<Person>();

            Conference conference = new Conference(
                "Test Title",
                "Test description",
                mockConferenceRoom.Object,
                DateTime.Now,
                DateTime.Now.AddHours(1),
                false,
                EventStatus.Planned,
                mockSpeaker.Object
                );

            conference.RegisterAttendee(mockAttendee.Object);

            // Act
            bool isRegistered = conference.IsAttendeeRegistered(mockAttendee.Object);

            // Assert
            Assert.IsTrue(isRegistered, "Attendee should be registered.");
        }

        /// <summary>
        /// Tests if IsAttendeeRegistered returns false when the attendee is not registered.
        /// </summary>
        [TestMethod]
        public void IsAttendeeRegistered_ReturnsFalse()
        {
            // Arrange
            Mock<ConferenceRoom> mockConferenceRoom = new Mock<ConferenceRoom>();
            mockConferenceRoom.Setup(room => room.NumberOfSeats).Returns(10);
            Mock<Person> mockSpeaker = new Mock<Person>();
            Mock<Person> mockAttendee = new Mock<Person>();

            Conference conference = new Conference(
                "Test Title",
                "Test description",
                mockConferenceRoom.Object,
                DateTime.Now,
                DateTime.Now.AddHours(1),
                false,
                EventStatus.Planned,
                mockSpeaker.Object
                );

            // Act
            bool isRegistered = conference.IsAttendeeRegistered(mockAttendee.Object);

            // Assert
            Assert.IsFalse(isRegistered, "Attendee should not be registered.");
        }
    }
}
