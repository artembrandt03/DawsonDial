using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using Moq;

namespace DawsonDialTests
{
    /// <summary>
    /// Tests the Course class.
    /// </summary>
    [TestClass]
    public class CourseTests
    {
        /// <summary>
        /// Tests that the constructor initializes properties correctly.
        /// </summary>
        [TestMethod]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            string courseCode = "Test course code";
            string subject = "Test subject";
            var mockTeacher = new Mock<Teacher>().Object;
            var mockSection = new Mock<Section>().Object;

            var teachers = new HashSet<Teacher> { mockTeacher };
            var sections = new HashSet<Section> { mockSection };

            // Act
            Course course = new Course(courseCode, subject, teachers, sections);

            // Assert
            Assert.AreEqual(courseCode, course.CourseCode, "Course codes not initialized correctly.");
            Assert.AreEqual(subject, course.Subject, "Course subjects not initialized correctly.");
            Assert.AreEqual(teachers, course.Teachers, "Course teachers not initialized correctly.");
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException when the course code parameter is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullCourseCode_ThrowsArgumentNullException()
        {
            // Arrange & Act
            Course course = new Course(
                null!,
                "Test subject",
                new HashSet<Teacher>(),
                new HashSet<Section>());
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException when the subject parameter is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullSubject_ThrowsArgumentNullException()
        {
            // Arrange & Act
            Course course = new Course(
                "Test course code",
                null!,
                new HashSet<Teacher>(),
                new HashSet<Section>());
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException when the teachers parameter is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullTeachers_ThrowsArgumentNullException()
        {
            // Arrange & Act
            Course course = new Course(
                "Test course code",
                "Test subject",
                null!,
                new HashSet<Section>());
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException when the rooms parameter is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullRooms_ThrowsArgumentNullException()
        {
            // Arrange & Act
            Course course = new Course(
                "Test course code",
                "Test subject",
                new HashSet<Teacher>(),
                null!);
        }
    }
}
