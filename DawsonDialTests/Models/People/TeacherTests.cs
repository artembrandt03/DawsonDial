namespace DawsonDialTests
{
    using System;
    using DawsonDial.Models.People;
    using DawsonDial.Models.Rooms;
    using DawsonDial.Models.Events;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class TeacherTests
    {
        /// <summary>
        /// Tests that Events and Classes collections are initialized.
        /// </summary>
        [TestMethod]
        public void Test_TeacherConstructor_ShouldInitializeCollections()
        {
            // Arrange
            Mock<Office> mockOffice = new();
            HashSet<Section> emptySections = new();
            Mock<OfficeHours> mockOfficeHours = new();

            // Act
            Teacher antoine = new(
                "antoine@dawsoncollege.qc.ca",
                "HelloKitty",
                "antoine",
                "oparin",
                35,
                false,
                "Experienced teacher",
                "Math",
                emptySections,
                mockOffice.Object,
                mockOfficeHours.Object);

            // Assert
            Assert.IsNotNull(antoine.Events);
            Assert.IsNotNull(antoine.Classes);
            Assert.AreEqual(0, antoine.Events.Count);
            Assert.AreEqual(0, antoine.Classes.Count);
        }

        /// <summary>
        /// Tests that OfficeRoom is properly assigned.
        /// </summary>
        [TestMethod]
        public void Test_Office_ShouldNotBeNull()
        {
            // Arrange
            var mockOffice = new Mock<Office>().Object;
            HashSet<Section> emptySections = new();
            Mock<OfficeHours> mockOfficeHours = new();

            // Act
            Teacher antoine = new(
                "antoine@dawsoncollege.qc.ca",
                "HelloKitty",
                "antoine",
                "oparin",
                35,
                false,
                "Experienced teacher",
                "Math",
                emptySections,
                mockOffice,
                mockOfficeHours.Object);

            // Assert
            Assert.IsNotNull(antoine.OfficeRoom);
            Assert.AreEqual(mockOffice, antoine.OfficeRoom);
        }

        /// <summary>
        /// Tests that adding a class increases the Classes list count.
        /// </summary>
        [TestMethod]
        public void Test_AddClass_ShouldUpdateClassesList()
        {
            // Arrange
            Mock<Office> mockOffice = new ();
            HashSet<Section> emptySections = new();
            Mock<OfficeHours> mockOfficeHours = new();

            Teacher antoine = new(
                "antoine@dawsoncollege.qc.ca",
                "HelloKitty",
                "antoine",
                "oparin",
                35,
                false,
                "Experienced teacher",
                "Math",
                emptySections,
                mockOffice.Object,
                mockOfficeHours.Object);

            var mockSection = new Mock<Section>().Object;

            // Act
            antoine.AddClass(mockSection);

            // Assert
            Assert.AreEqual(1, antoine.Classes.Count);
        }

        /// <summary>
        /// Tests that removing a class should update the classes list.
        /// </summary>
        [TestMethod]
        public void Test_RemoveClass_ShouldUpdateClassesList()
        {
            Mock<Office> mockOffice = new ();
            HashSet<Section> emptySections = new();
            Mock<OfficeHours> mockOfficeHours = new();

            Teacher antoine = new(
                "antoine@dawsoncollege.qc.ca",
                "HelloKitty",
                "antoine",
                "oparin",
                35,
                false,
                "Experienced teacher",
                "Math",
                emptySections,
                mockOffice.Object,
                mockOfficeHours.Object);

            Mock<Section> mockSection = new();

            antoine.AddClass(mockSection.Object);
            antoine.RemoveClass(mockSection.Object);

            Assert.AreEqual(0, antoine.Classes.Count);
        }

        /// <summary>
        /// Tests that clearing classes should empty the class list.
        /// </summary>
        [TestMethod]
        public void Test_ClearClasses_ShouldEmptyClasses()
        {
            Mock<Office> mockOffice = new();
            HashSet<Section> emptySections = new();
            Mock<OfficeHours> mockOfficeHours = new();

            Teacher antoine = new(
                "antoine@dawsoncollege.qc.ca",
                "HelloKitty",
                "antoine",
                "oparin",
                35,
                false,
                "Experienced teacher",
                "Math",
                emptySections,
                mockOffice.Object,
                mockOfficeHours.Object);

            Mock<Section> mockSection = new();

            antoine.AddClass(mockSection.Object);
            antoine.ClearClasses();

            Assert.AreEqual(0, antoine.Classes.Count);
        }
    }
}
