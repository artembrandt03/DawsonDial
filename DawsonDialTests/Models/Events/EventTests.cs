using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.Enums;
using Moq;

namespace DawsonDialTests
{
    /// <summary>
    /// Tests the Event class.
    /// </summary>
    [TestClass]
    public class EventTests
    {
        /// <summary>
        /// Tests that an ArgumentException is thrown when the event title is null or empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EventTitle_ShouldNotBeEmptyOrNull()
        {
            // Arrange
            string? nullTitle = null;
            Mock<Office> mockOffice = new Mock<Office>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            // Act
            OfficeMeeting eventInstance = new OfficeMeeting(
                nullTitle!, // null title
                "Testing to see if exception is thrown",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);
        }

        /// <summary>
        /// Tests that an ArgumentException is thrown when the event description is null or empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EventDescription_ShouldNotBeEmptyOrNull()
        {
            // Arrange
            string? nullDescription = null;
            Mock<Office> mockOffice = new Mock<Office>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            // Act
            OfficeMeeting eventInstance = new OfficeMeeting(
                "Test Title",
                nullDescription!, // null description
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);
        }

        /// <summary>
        /// Tests that an ArgumentNullException is thrown when the event location is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_EventLocation_ShouldNotBeNull()
        {
            // Arrange
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            // Act
            OfficeMeeting eventInstance = new OfficeMeeting(
                "Test Title",
                "Test to see if exception is thrown",
                null!, // null location
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);

        }

        /// <summary>
        /// Tests that an ArgumentException is thrown when the start date time is after the end date time.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_StartDateTime_ShouldBeBeforeEndDateTime()
        {
            // Arrange
            Mock<Office> mockOffice = new Mock<Office>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            // Act
            OfficeMeeting eventInstance = new OfficeMeeting(
                "Test Title",
                "Test to see if exception is thrown",
                mockOffice.Object,
                DateTime.Now.AddMinutes(30), // startDateTime after endDateTime
                DateTime.Now,
                false,
                EventStatus.Planned,
                mockTeacher.Object);
        }

        /// <summary>
        /// Tests that adding a null participant throws an ArgumentNullException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddParticipant_NullParticipant_ShouldThrowArgumentNullException()
        {
            // Arrange
            Mock<Office> mockOffice = new Mock<Office>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            OfficeMeeting eventInstance = new OfficeMeeting(
                "Test Title",
                "Test to see if exception is thrown",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);

            // Act
            eventInstance.AddParticipant(null!);
        }

        /// <summary>
        /// Test that adding a participant to an event the IsFull throws an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddParticipant_EventFull_ShouldThrowInvalidOperationException()
        {
            // Arrange
            // Make mock office and give it 1 seat
            Mock<Office> mockOffice = new Mock<Office>();
            mockOffice.Setup(office => office.NumberOfSeats).Returns(1);

            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Person> mockPerson = new Mock<Person>();

            OfficeMeeting eventInstance = new OfficeMeeting(
                "Test Title",
                "Test description",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);

            // Act
            eventInstance.AddParticipant(mockTeacher.Object);
            eventInstance.AddParticipant(mockPerson.Object); // should throw exception
        }

        /// <summary>
        /// Tests that adding a participant that is already added throws an InvalidOperationException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddParticipant_ParticipantAlreadyAdded_ShouldThrowInvalidOperationException()
        {
            // Arrange
            Mock<Office> mockOffice = new Mock<Office>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            OfficeMeeting eventInstance = new OfficeMeeting(
                "Test Title",
                "Test to see if exception is thrown",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);

            // Act
            eventInstance.AddParticipant(mockTeacher.Object);
            eventInstance.AddParticipant(mockTeacher.Object); // should throw exception
        }

        /// <summary>
        /// Tests that a participant is actually added to the event.
        /// </summary>
        [TestMethod]
        public void AddParticipant_ParticipantIsAdded()
        {
            // Arrange
            // Make mock office and give it 10 seats
            Mock<Office> mockOffice = new Mock<Office>();
            mockOffice.Setup(office => office.NumberOfSeats).Returns(10);

            // Make mock person and give them a test username
            Mock<Person> mockPerson = new Mock<Person>();
            mockPerson.Setup(person => person.Username).Returns("TestUser");

            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            OfficeMeeting eventInstance = new OfficeMeeting(
                "Test Title",
                "Test description",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);

            // Act
            eventInstance.AddParticipant(mockPerson.Object);

            // Assert
            // Check participant is added via Username comparison
            Assert.IsTrue(eventInstance.Participants.Any(person => person.Username == "TestUser"),
                            "Participant was not added to the event.");
            // Check event participants count is 1
            Assert.AreEqual(1, eventInstance.Participants.Count, "Event participants count should be 1.");
        }

        /// <summary>
        /// Tests the removal of a participant from an event.
        /// </summary>
        [TestMethod]
        public void RemoveParticipant_ParticipantIsRemoved()
        {
            // Arrange
            Mock<Office> mockOffice = new Mock<Office>();
            mockOffice.Setup(office => office.NumberOfSeats).Returns(10);

            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            mockTeacher.Setup(teacher => teacher.Username).Returns("TestTeacher");

            OfficeMeeting eventInstance = new OfficeMeeting(
                "Test Title",
                "Test description",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);

            // Act
            eventInstance.AddParticipant(mockTeacher.Object);
            eventInstance.RemoveParticipant(mockTeacher.Object);

            // Assert
            // Check participant is removed via Username comparison
            Assert.IsFalse(eventInstance.Participants.Any(person => person.Username == "TestTeacher"),
                            "Event should not have the specified participant.");
            // Check event participants count is 0
            Assert.AreEqual(0, eventInstance.Participants.Count, "Event participants count should be 0.");
        }

        /// <summary>
        /// Tests that RemoveParticipant throws an ArgumentNullException when the participant is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveParticipant_ParticipantIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            Mock<Office> mockOffice = new Mock<Office>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            OfficeMeeting eventInstance = new OfficeMeeting(
                "Test Title",
                "Test description",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);

            // Act
            eventInstance.RemoveParticipant(null!);
        }

        /// <summary>
        /// Tests that HasParticipant returns true when the participant is added.
        /// </summary>
        [TestMethod]
        public void HasParticipant_ReturnsTrue()
        {
            // Arrange
            Mock<Office> mockOffice = new Mock<Office>();
            mockOffice.Setup(office => office.NumberOfSeats).Returns(1);

            Mock<Person> mockPerson = new Mock<Person>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            OfficeMeeting eventInstance = new OfficeMeeting(
                "Test Title",
                "Test description",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);

            // Act
            eventInstance.AddParticipant(mockPerson.Object);

            // Assert
            Assert.IsTrue(eventInstance.HasParticipant(mockPerson.Object),
                            "Event should have the specified participant.");
        }

        /// <summary>
        /// Tests that HasParticipant returns false when the event does not have the specified participant.
        /// </summary>
        [TestMethod]
        public void HasParticipant_ReturnsFalse()
        {
            // Arrange
            Mock<Office> mockOffice = new Mock<Office>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Person> mockPerson = new Mock<Person>();

            OfficeMeeting eventInstance = new OfficeMeeting(
                "Test Title",
                "Test description",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);

            // Act
            // Do not add participant

            // Assert
            Assert.IsFalse(eventInstance.HasParticipant(mockPerson.Object),
                            "Event should not have the specified participant.");
        }

        /// <summary>
        /// Tests that HasParticipant throws ArgumentNullException when the participant is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasParticipant_ParticipantIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            Mock<Office> mockOffice = new Mock<Office>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            OfficeMeeting eventInstance = new OfficeMeeting(
                "Test Title",
                "Test description",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);

            // Act
            eventInstance.HasParticipant(null!);
        }
    }
}
