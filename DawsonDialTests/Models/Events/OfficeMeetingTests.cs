using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.Enums;
using Moq;

namespace DawsonDialTests
{
    /// <summary>
    /// Tests the OfficeMeeting class.
    /// </summary>
    [TestClass]
    public class OfficeMeetingTests
    {
        /// <summary>
        /// Tests that the constructor initializes properties correctly.
        /// </summary>
        [TestMethod]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            string title = "Test Title";
            string description = "Test description";
            Mock<Office> mockOffice = new Mock<Office>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddMinutes(30);
            bool isRecurring = false;
            EventStatus status = EventStatus.Planned;

            // Act
            OfficeMeeting officeMeeting = new OfficeMeeting(
                title,
                description,
                mockOffice.Object,
                startTime,
                endTime,
                isRecurring,
                status,
                mockTeacher.Object);

            // Assert
            Assert.AreEqual(title, officeMeeting.Title, "Title not initialized correctly.");
            Assert.AreEqual(description, officeMeeting.Description, "Description not initialized correctly.");
            Assert.AreEqual(mockOffice.Object, officeMeeting.Room, "Room not initialized correctly.");
            Assert.AreEqual(startTime, officeMeeting.StartDateTime, "Start time not initialized correctly.");
            Assert.AreEqual(endTime, officeMeeting.EndDateTime, "End time not initialized correctly.");
            Assert.AreEqual(isRecurring, officeMeeting.IsRecurring, "IsRecurring value not initialized correctly.");
            Assert.AreEqual(status, officeMeeting.Status, "Status not initialized correctly.");
            Assert.AreEqual(mockTeacher.Object, officeMeeting.Teacher, "Teacher not initialized correctly.");
            Assert.IsNotNull(officeMeeting.Participants, "Participants should not be null.");
            Assert.AreEqual(0, officeMeeting.Participants.Count, "Participants should be empty.");
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException when the teacher is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_TeacherIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            Mock<Office> mockOffice = new Mock<Office>();

            // Act
            OfficeMeeting officeMeeting = new OfficeMeeting(
                "Test Title",
                "Test description",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                null!);
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentException when the duration is too long.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_TooLongDuration_ShouldThrowArgumentException()
        {
            // Arrange
            Mock<Office> mockOffice = new Mock<Office>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            // Act
            OfficeMeeting officeMeeting = new OfficeMeeting(
                "Test Title",
                "Test description",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(120),
                false,
                EventStatus.Planned,
                mockTeacher.Object);
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentException when the duration is too short.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_TooShortDuration_ShouldThrowArgumentException()
        {
            // Arrange
            Mock<Office> mockOffice = new Mock<Office>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();

            // Act
            OfficeMeeting officeMeeting = new OfficeMeeting(
                "Test Title",
                "Test description",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(7),
                false,
                EventStatus.Planned,
                mockTeacher.Object);
        }

        /// <summary>
        /// Test that AddStudent adds a student to participants.
        /// </summary>
        [TestMethod]
        public void AddStudent_ShouldAddStudentToParticipants()
        {
            // Arrange
            Mock<Office> mockOffice = new Mock<Office>();
            mockOffice.Setup(office => office.NumberOfSeats).Returns(5);
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Student> mockStudent = new Mock<Student>();

            OfficeMeeting officeMeeting = new OfficeMeeting(
                "Test Title",
                "Test description",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);

            // Act
            officeMeeting.AddStudent(mockStudent.Object);

            // Assert
            Assert.AreEqual(1, officeMeeting.Participants.Count,
                            "Participants should have one participant.");
            Assert.IsTrue(officeMeeting.Participants.Contains(mockStudent.Object),
                            "Added student should be in the participants list.");
        }

        /// <summary>
        /// Tests that removing a student from an office meeting removes them from the participants list.
        /// </summary>
        [TestMethod]
        public void RemoveStudent_ShouldRemoveStudentFromParticipants()
        {
            // Arrange
            Mock<Office> mockOffice = new Mock<Office>();
            mockOffice.Setup(office => office.NumberOfSeats).Returns(5);
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Student> mockStudent = new Mock<Student>();

            OfficeMeeting officeMeeting = new OfficeMeeting(
                "Test Title",
                "Test description",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);

            officeMeeting.AddStudent(mockStudent.Object);

            // Act
            officeMeeting.RemoveStudent(mockStudent.Object);

            // Assert
            Assert.AreEqual(0, officeMeeting.Participants.Count,
                            "Participants should have no participants.");
            Assert.IsFalse(officeMeeting.Participants.Contains(mockStudent.Object),
                            "Removed student should not be in the participants list.");
        }

        /// <summary>
        /// Tests if HasStudent returns true when student is in the participants list.
        /// </summary>
        [TestMethod]
        public void HasStudent_ShouldReturnTrueIfStudentIsInParticipants()
        {
            // Arrange
            Mock<Office> mockOffice = new Mock<Office>();
            mockOffice.Setup(office => office.NumberOfSeats).Returns(5);
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Student> mockStudent = new Mock<Student>();

            OfficeMeeting officeMeeting = new OfficeMeeting(
                "Test Title",
                "Test description",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);

            officeMeeting.AddStudent(mockStudent.Object);

            // Act
            bool hasStudent = officeMeeting.HasStudent(mockStudent.Object);

            // Assert
            Assert.IsTrue(hasStudent, "Office meeting should have the student.");
        }

        /// <summary>
        /// Tests if HasStudent returns false when student is not in the participants list.
        /// </summary>
        [TestMethod]
        public void HasStudent_ShouldReturnFalseIfStudentIsNotInParticipants()
        {
            // Arrange
            Mock<Office> mockOffice = new Mock<Office>();
            mockOffice.Setup(office => office.NumberOfSeats).Returns(5);
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Student> mockStudent = new Mock<Student>();

            OfficeMeeting officeMeeting = new OfficeMeeting(
                "Test Title",
                "Test description",
                mockOffice.Object,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                EventStatus.Planned,
                mockTeacher.Object);

            // Act
            bool hasStudent = officeMeeting.HasStudent(mockStudent.Object);

            // Assert
            Assert.IsFalse(hasStudent, "Office meeting should not have the student.");
        }
    }
}
