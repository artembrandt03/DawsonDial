using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using Moq;

namespace DawsonDialTests
{
    [TestClass]
    public class StudentTests
    {
        /// <summary>
        /// Tests that Events and Classes collections are initialized.
        /// </summary>
        [TestMethod]
        public void Test_StudentConstructor_ShouldInitializeCollections()
        {
            // Arrange
            HashSet<Section> emptySections = [];

            // Act
            Student antoine = new(
                "antoine@dawsoncollege.qc.ca",
                "HelloKitty",
                "Antoine",
                "Oparin",
                19,
                false,
                "Chill dude",
                2333239,
                "Computer Science",
                2,
                emptySections);

            // Assert
            Assert.IsNotNull(antoine.Events);
            Assert.IsNotNull(antoine.Classes);
            Assert.AreEqual(0, antoine.Events.Count);
            Assert.AreEqual(0, antoine.Classes.Count);
        }

        /// <summary>
        /// Tests that an ArgumentException is thrown when StudentID is not 7 digits.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_StudentID_ShouldFollowCorrectFormat()
        {
            // Arrange
            HashSet<Section> emptySections = [];

            // Act
            Student antoine = new(
                "antoine@dawsoncollege.qc.ca",
                "HelloKitty",
                "Antoine",
                "Oparin",
                19,
                false,
                "Chill dude",
                1234, // Invalid StudentID (not 7 digits)
                "Computer Science",
                2,
                emptySections);
        }

        /// <summary>
        /// Tests that an ArgumentException is thrown when Year is not in the range 1-3.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Year_ShouldBeInValidRange()
        {
            // Arrange
            HashSet<Section> emptySections = [];

            // Act
            Student antoine = new(
                "antoine@dawsoncollege.qc.ca",
                "HelloKitty",
                "Antoine",
                "Oparin",
                19,
                false,
                "Chill dude",
                2333239,
                "Computer Science",
                5, // Invalid year
                emptySections);
        }

        /// <summary>
        /// Tests that adding a class increases the Classes list count.
        /// </summary>
        [TestMethod]
        public void Test_AddClass_ShouldUpdateClassesList()
        {
            // Arrange
            HashSet<Section> emptySections = [];
            Student antoine = new(
                "antoine@dawsoncollege.qc.ca",
                "HelloKitty",
                "Antoine",
                "Oparin",
                19,
                false,
                "Chill dude",
                2333239,
                "Computer Science",
                2,
                emptySections);

            // Create a mock section with EnrolledStudents property
            var mockSection = new Mock<Section>();
            mockSection.Setup(s => s.EnrolledStudents).Returns(new HashSet<Student>());

            // Act
            antoine.AddClass(mockSection.Object);

            // Assert
            Assert.AreEqual(1, antoine.Classes.Count);
        }
    }
}
