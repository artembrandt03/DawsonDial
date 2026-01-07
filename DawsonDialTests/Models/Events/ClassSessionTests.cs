using DawsonDial.Models.Events;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.Enums;
using Moq;

namespace DawsonDialTests
{
    /// <summary>
    /// Tests the ClassSession class.
    /// </summary>
    [TestClass]
    public class ClassSessionTests
    {
        /// <summary>
        /// Tests that the constructor initializes properties correctly.
        /// </summary>
        [TestMethod]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            Mock<Room> mockClassroom = new Mock<Room>();
            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();

            string title = "Test Title";
            string description = "Test description";
            string semester = "Winter/2025";
            string section = "0001";
            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.Now.AddHours(1);
            bool isRecurring = true;
            EventStatus status = EventStatus.InProgress;

            // Act
            ClassSession classSession = new ClassSession(
                title,
                description,
                mockClassroom.Object,
                mockCourse.Object,
                semester,
                section,
                startTime,
                endTime,
                isRecurring,
                status,
                mockSchedule.Object);

            // Assert
            Assert.AreEqual(title, classSession.Title, "Title not initialized correctly.");
            Assert.AreEqual(description, classSession.Description, "Description not initialized correctly.");
            Assert.AreEqual(mockClassroom.Object, classSession.Room, "Room not initialized correctly.");
            Assert.AreEqual(mockCourse.Object, classSession.Course, "Course not initialized correctly.");
            Assert.AreEqual(semester, classSession.Semester, "Semester not initialized correctly.");
            Assert.AreEqual(section, classSession.Section, "Section not initialized correctly.");
            Assert.AreEqual(startTime, classSession.StartDateTime, "StartDateTime not initialized correctly.");
            Assert.AreEqual(endTime, classSession.EndDateTime, "EndDateTime not initialized correctly.");
            Assert.AreEqual(isRecurring, classSession.IsRecurring, "IsRecurring not initialized correctly.");
            Assert.AreEqual(status, classSession.Status, "Status not initialized correctly.");
            Assert.AreEqual(mockSchedule.Object, classSession.Schedule, "Schedule not initialized correctly.");
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentException when the section is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullSection_ThrowsArgumentException()
        {
            // Arrange
            Mock<Room> mockClassroom = new Mock<Room>();
            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();

            // Act
            ClassSession classSession = new ClassSession(
                "Test Title",
                "Test description",
                mockClassroom.Object,
                mockCourse.Object,
                "Winter/2025",
                null!,
                DateTime.Now,
                DateTime.Now.AddHours(1),
                true,
                EventStatus.InProgress,
                mockSchedule.Object);
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentException when the semester is empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptySemester_ThrowsArgumentException()
        {
            // Arrange
            Mock<Room> mockClassroom = new Mock<Room>();
            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();

            // Act
            ClassSession classSession = new ClassSession(
                "Test Title",
                "Test description",
                mockClassroom.Object,
                mockCourse.Object,
                string.Empty,
                "0001",
                DateTime.Now,
                DateTime.Now.AddHours(1),
                true,
                EventStatus.InProgress,
                mockSchedule.Object);
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentException when the section is empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptySection_ThrowsArgumentException()
        {
            // Arrange
            Mock<Room> mockClassroom = new Mock<Room>();
            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();

            // Act
            ClassSession classSession = new ClassSession(
                "Test Title",
                "Test description",
                mockClassroom.Object,
                mockCourse.Object,
                "Winter/2025",
                string.Empty,
                DateTime.Now,
                DateTime.Now.AddHours(1),
                true,
                EventStatus.InProgress,
                mockSchedule.Object);
        }

        /// <summary>
        /// Tests the ToString method outputs as expected.
        /// </summary>
        [TestMethod]
        public void ToString_ReturnsExpectedFormat()
        {
            // Arrange
            Mock<Room> mockClassroom = new Mock<Room>();
            Mock<Course> mockCourse = new Mock<Course>();
            mockCourse.Setup(course => course.CourseCode).Returns("testcode");
            Mock<Schedule> mockSchedule = new Mock<Schedule>();

            // Act
            ClassSession classSession = new ClassSession(
                "Test Title",
                "Test description",
                mockClassroom.Object,
                mockCourse.Object,
                "Winter/2025",
                "0001",
                DateTime.Now,
                DateTime.Now.AddHours(1),
                true,
                EventStatus.InProgress,
                mockSchedule.Object);

            // Assert
            string expected = "testcode - Test Title (Winter/2025 - 0001)";
            Assert.AreEqual(expected, classSession.ToString(),
                            "Output string not formatted as expected.");
        }
    }
}
