using DawsonDial.Models.Enums;
using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;
using Moq;

namespace DawsonDialTests
{
    /// <summary>
    /// Tests the Person class.
    /// </summary>
    [TestClass]
    public class PersonTests
    {
        /// <summary>
        /// Tests that an ArgumentException is thrown when the username isn't in the correct format.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_Username_ShouldBeValid()
        {
            // Arrange
            HashSet<Section> emptySections = [];

            // Act
            Student antoine = new("antoine@gmail.com", "HelloKitty", "Antoine", "Oparin", 19, false, "Chill dude", 2333239, "Computer Science", 2, emptySections);
        }
        /// <summary>
        /// Tests that an ArgumentException is thrown when the password is empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_Password_ShouldNotBeEmpty()
        {
            // Arrange
            HashSet<Section> emptySections = [];

            // Act
            Student antoine = new("antoine@dawsoncollege.qc.ca", "", "Antoine", "Oparin", 19, false, "Chill dude", 2333239, "Computer Science", 2, emptySections);
        }
        /// <summary>
        /// Tests that an ArgumentException is thrown when the age isn't in range 10 - 100.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Age_ShouldBeInRange()
        {
            // Arrange
            HashSet<Section> emptySections = [];

            // Act
            Student antoine = new("antoine@dawsoncollege.qc.ca", "HelloKitty", "Antoine", "Oparin", -3, false, "Chill dude", 2333239, "Computer Science", 2, emptySections);
        }

        /// <summary>
        /// Tests that an ArgumentException is thrown when the age isn't in range 10 - 100.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Year_ShouldBeInRange()
        {
            // Arrange
            HashSet<Section> emptySections = [];

            // Act
            Student antoine = new("antoine@dawsoncollege.qc.ca", "HelloKitty", "Antoine", "Oparin", 19, false, "Chill dude", 2333239, "Computer Science", -2, emptySections);
        }

        /// <summary>
        /// Tests that an ArgumentException is thrown when adding Null event to person object.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEvent_ShouldNotAddNullEvent()
        {
            // Arrange
            HashSet<Section> emptySections = [];
            Student antoine = new("antoine@dawsoncollege.qc.ca", "HelloKitty", "Antoine", "Oparin", 19, false, "Chill dude", 2333239, "Computer Science", 2, emptySections);

            // Act
            antoine.AddEvent(null!);
        }

        /// <summary>
        /// Tests that an ArgumentException is thrown when removing a nonexistent event from a person object.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveEvent_ShouldNotRemoveNonexistentEvent()
        {
            // Arrange
            HashSet<Section> emptySections = [];
            Student antoine = new("antoine@dawsoncollege.qc.ca", "HelloKitty", "Antoine", "Oparin", 19, false, "Chill dude", 2333239, "Computer Science", 2, emptySections);

            // Act
            antoine.RemoveEvent(null!);
        }

        /// <summary>
        /// Tests if ClearEvents method clears the Events HashSet.
        /// </summary>
        [TestMethod]
        public void ClearEvent_ShouldClearEventsHashSet()
        {
            // Arrange
            HashSet<Section> emptySections = [];
            Mock<Office> mockOffice = new ();
            Mock<Teacher> mockTeacher = new ();
            Student antoine = new("antoine@dawsoncollege.qc.ca", "HelloKitty", "Antoine", "Oparin", 19, false, "Chill dude", 2333239, "Computer Science", 2, emptySections);
            OfficeMeeting eventInstance = new ("Test Title", "Test description", mockOffice.Object, DateTime.Now, DateTime.Now.AddMinutes(30), false, EventStatus.Planned, mockTeacher.Object);


            // Act
            antoine.AddEvent(eventInstance);
            antoine.ClearEvents();

            // Assert
            Assert.AreEqual(0, antoine.Events.Count);
        }
    }
}
