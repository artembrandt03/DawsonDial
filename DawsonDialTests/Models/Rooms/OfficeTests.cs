// Test_OfficeConstructor_ShouldInitializeProperties: Properties set correctly
// Test_OfficeConstructor_InitializesEmptyTeachersList: AssignedTeachers initialized
// Test_AssignTeacher_ShouldAddToAssignedTeachers: Adding teacher increases list count
using DawsonDial.Models.Rooms;
using DawsonDial.Models.People;
using System.Collections.Generic;
using DawsonDial.Models.Events;
using Moq;

namespace DawsonDialTests.Models.Rooms
{
    [TestClass]
    public class OfficeTests
    {
        //1: Test if the constructor initializes properties correctly and as expected
        [TestMethod]
        public void Test_OfficeConstructor_ShouldInitializeProperties()
        {
            //Arrange
            string roomNumber = "4E.10";
            int seats = 3;
            bool isShared = true;

            //Act
            Office office = new Office(roomNumber, seats, isShared);

            //Assert
            Assert.AreEqual(roomNumber, office.RoomNumber);
            Assert.AreEqual(seats, office.NumberOfSeats);
            Assert.AreEqual(isShared, office.Shared);
            Assert.IsTrue(office.IsAvailable, "new office should be available by default");
        }

        //2: Test if AssignedTeachers is initialized as an empty list at first
        [TestMethod]
        public void Test_OfficeConstructor_InitializesEmptyTeachersList()
        {
            //Arrange & Act
            Office office = new Office("4E.10", 1, false);

            //Assert
            Assert.IsNotNull(office.AssignedTeachers);
            Assert.AreEqual(0, office.AssignedTeachers.Count, "list of assigned teachers should be empty");
        }

        //3: Test if AssignTeacher correctly adds a teacher
        [TestMethod]
        public void Test_AssignTeacher_ShouldAddToAssignedTeachers()
        {
            // Arrange
            Office office = new Office("2F.16", 1, false);

            // Create mock teacher instead of actual instance
            var mockTeacher = new Mock<Teacher>();
            mockTeacher.Setup(t => t.Username).Returns("mahsasedeghi@dawsoncollege.qc.ca");
            mockTeacher.Setup(t => t.Password).Returns("securePass123");
            mockTeacher.Setup(t => t.FirstName).Returns("Mahsa");
            mockTeacher.Setup(t => t.LastName).Returns("Sadeghi");
            mockTeacher.Setup(t => t.Department).Returns("Computer Science");

            // Act
            office.AssignTeacher(mockTeacher.Object);

            // Assert
            Assert.AreEqual(1, office.AssignedTeachers.Count,
                "assigning a teacher should increase the list count by 1");
            Assert.AreSame(mockTeacher.Object, office.AssignedTeachers[0],
                "The assigned teacher should match the input");
        }

        //4: Test if reserving an office makes it unavailable to reserve againb
        [TestMethod]
        public void Test_ReserveRoom_ShouldMakeUnavailable()
        {
            //Arrange
            Office office = new Office("2F.16", 3, true);

            //Act
            office.ReserveRoom();

            //Assert
            Assert.IsFalse(office.IsAvailable, "reserving an office should make it unavailable");
        }

        //5: Test if releasing an office makes it available again
        [TestMethod]
        public void Test_ReleaseRoom_ShouldMakeAvailable()
        {
            // Arrange
            Office office = new Office("2F.16", 6, true);
            office.ReserveRoom(); // Make it unavailable first

            //Act
            office.ReleaseRoom();

            //Assert
            Assert.IsTrue(office.IsAvailable, "Releasing an office should make it available");
        }

        //6: Test if Shared property is correctly assigned
        [TestMethod]
        public void Test_SharedProperty_ShouldSetCorrectly()
        {
            //Arrange
            Office sharedOffice = new Office("2F.16", 3, true);
            Office privateOffice = new Office("2F.24", 1, false);

            //Assert
            Assert.IsTrue(sharedOffice.Shared, "should be true for shared offices");
            Assert.IsFalse(privateOffice.Shared, "should be false for private offices");
        }
    }
}
