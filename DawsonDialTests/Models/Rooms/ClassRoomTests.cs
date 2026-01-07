// Test_ClassRoomConstructor_ShouldInitializeProperties: Properties set correctly
// Test_ClassRoomConstructor_InitializesEmptyClassesList: AssignedClasses initialized
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.Events;
using DawsonDial.Models.Enums;
using DawsonDial.Models.People;
using Moq;

namespace DawsonDialTests.Models.Rooms
{
    [TestClass]
    public class ClassRoomTests
    {
        //1: Test if the constructor initializes properties correctly
        [TestMethod]
        public void Test_ClassRoomConstructor_ShouldInitializeProperties()
        {
            //Arrange
            string roomNumber = "2F24";
            int seats = 30;
            bool hasProjector = true;
            bool hasComputers = false;

            //Act
            ClassRoom classRoom = new ClassRoom(roomNumber, seats, hasProjector, hasComputers);

            //Assert
            Assert.AreEqual(roomNumber, classRoom.RoomNumber);
            Assert.AreEqual(seats, classRoom.NumberOfSeats);
            Assert.AreEqual(hasProjector, classRoom.HasProjector);
            Assert.AreEqual(hasComputers, classRoom.HasComputers);
            Assert.IsTrue(classRoom.IsAvailable, "new classroom should be available by default");
        }

        //2: Test if AssignedClasses is initialized as an empty list at first
        [TestMethod]
        public void Test_ClassRoomConstructor_InitializesEmptyClassesList()
        {
            //Arrange & Act
            ClassRoom classRoom = new ClassRoom("2F24", 30, false, true);

            //Assert
            Assert.IsNotNull(classRoom.AssignedClasses);
            Assert.AreEqual(0, classRoom.AssignedClasses.Count, "assignedClasses list should be empty on initialization");
        }

        //3: Test if AssignClass correctly adds a ClassSession object
        //This test method uses mock mockCourse, since Course is an abstract class
        //and cannot be instanciated directly.
        [TestMethod]
        public void Test_AssignClass_ShouldAddToAssignedClassesList()
        {
            //Arrange
            var classRoom = new ClassRoom("2F.24", 40, true, true);

            //Creates a mock Schedule
            var timeSlots = new Dictionary<DayOfWeek, (TimeOnly, TimeOnly)>
            {
                { DayOfWeek.Monday, (new TimeOnly(9, 0), new TimeOnly(10, 30)) }
            };
            var schedule = new Schedule(timeSlots, new DateOnly(2025, 1, 22), new DateOnly(2025, 5, 27));

            //Mocking the interface instead of the abstract class
            var mockCourse = new Mock<Course>();
            mockCourse.Setup(c => c.CourseCode).Returns("CS404");

            //Create a valid ClassSession instance using the mocked Course
            var classSession = new ClassSession(
                title: "Intro to C#",
                description: "C# basics and comparisons to Java",
                room: classRoom,
                course: mockCourse.Object,
                semester: "Winter 2025",
                section: "001",
                startDateTime: DateTime.Now,
                endDateTime: DateTime.Now.AddHours(1),
                isRecurring: false,
                status: EventStatus.Planned,
                schedule: schedule
            );

            //Act
            classRoom.AssignClass(classSession);

            //Assert
            Assert.AreEqual(1, classRoom.AssignedClasses.Count, "Assigning a class should increase the list count.");
            Assert.AreSame(classSession, classRoom.AssignedClasses[0], "The assigned class should match the input.");
        }

        //4: Testing if reserving a classroom makes it unavailable
        [TestMethod]
        public void Test_ReserveRoom_ShouldMakeUnavailable()
        {
            //Arrange
            ClassRoom classRoom = new ClassRoom("2F.24", 30, false, false);

            //Act
            classRoom.ReserveRoom();

            //Assert
            Assert.IsFalse(classRoom.IsAvailable, "reserving a classroom should make it unavailable");
        }

        //5: Testing if releasing a classroom makes it available again
        [TestMethod]
        public void Test_ReleaseRoom_ShouldMakeAvailable()
        {
            //Arrange
            ClassRoom classRoom = new ClassRoom("2F.24", 30, true, false);
            classRoom.ReserveRoom(); //Make it unavailable first

            //Act
            classRoom.ReleaseRoom();

            //Assert
            Assert.IsTrue(classRoom.IsAvailable, "releasing a room should make it available");
        }

        //6: Test if HasProjector and HasComputers properties are correctly set and working
        [TestMethod]
        public void Test_ClassRoom_HasProjectorAndComputers()
        {
            //Arrange
            ClassRoom classroomWithBoth = new ClassRoom("2F.24", 35, true, true);
            ClassRoom classroomWithNeither = new ClassRoom("4E.09", 25, false, false);

            //Assert
            Assert.IsTrue(classroomWithBoth.HasProjector, "A lab should have a projector");
            Assert.IsTrue(classroomWithBoth.HasComputers, "A lab should have computers");
            Assert.IsFalse(classroomWithNeither.HasProjector, "Classroom should NOT have a projector");
            Assert.IsFalse(classroomWithNeither.HasComputers, "Classroom should NOT have computers");
        }
    }
}
