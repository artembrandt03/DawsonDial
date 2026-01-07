using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;
using Moq;

namespace DawsonDialTests
{
    /// <summary>
    /// Tests the Schedule class.
    /// </summary>
    [TestClass]
    public class ScheduleTests
    {
        /// <summary>
        /// Tests that the constructor initializes the properties correctly.
        /// </summary>
        [TestMethod]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var timeSlots = new Dictionary<DayOfWeek, (TimeOnly StartTime, TimeOnly EndTime)>
            {
                { DayOfWeek.Monday, (TimeOnly.FromTimeSpan(TimeSpan.Zero), TimeOnly.FromTimeSpan(TimeSpan.FromHours(8))) },
                { DayOfWeek.Tuesday, (TimeOnly.FromTimeSpan(TimeSpan.Zero), TimeOnly.FromTimeSpan(TimeSpan.FromHours(8))) },
                { DayOfWeek.Wednesday, (TimeOnly.FromTimeSpan(TimeSpan.Zero), TimeOnly.FromTimeSpan(TimeSpan.FromHours(8))) },
                { DayOfWeek.Thursday, (TimeOnly.FromTimeSpan(TimeSpan.Zero), TimeOnly.FromTimeSpan(TimeSpan.FromHours(8))) }
            };

            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

            // Act
            Schedule schedule = new Schedule(timeSlots, startDate, endDate);

            // Assert
            Assert.IsNotNull(schedule, "Schedule is null.");
            // ensure keys are the same
            Assert.AreEqual(timeSlots.Count, schedule.TimeSlots.Count, "Time Slots count does not match.");
            // ensure timeslots are the same
            foreach (var timeSlot in schedule.TimeSlots)
            {
                var expectedSlot = timeSlots[timeSlot.DayOfWeek];
                Assert.AreEqual(expectedSlot.StartTime, timeSlot.StartTime,
                    $"Start time not initialized correctly for {timeSlot.DayOfWeek}.");
                Assert.AreEqual(expectedSlot.EndTime, timeSlot.EndTime,
                    $"End time not initialized correctly for {timeSlot.DayOfWeek}.");
            }
            Assert.AreEqual(startDate, schedule.StartDate, "Start date not initialized correctly.");
            Assert.AreEqual(endDate, schedule.EndDate, "End date not initialized correctly.");
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentException when timeSlots is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_TimeSlotsNull_ThrowsArgumentException()
        {
            // Arrange & Act
            Schedule schedule = new Schedule(
                null!,
                DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentException when timeSlots is empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_TimeSlotsEmpty_ThrowsArgumentException()
        {
            // Arrange
            var timeSlots = new Dictionary<DayOfWeek, (TimeOnly StartTime, TimeOnly EndTime)>();

            // Act
            Schedule schedule = new Schedule(
                timeSlots,
                DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentException when a timeslot start time is after the end time.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_InvalidStartEndTimes_ThrowsArgumentException()
        {
            // Arrange
            var timeSlots = new Dictionary<DayOfWeek, (TimeOnly StartTime, TimeOnly EndTime)>();
            timeSlots.Add(DayOfWeek.Monday,
                (new TimeOnly(14, 00), // 2:00 PM
                 new TimeOnly(13, 00)  // 1:00 PM
                ));

            // Act
            Schedule schedule = new Schedule(
                timeSlots,
                new DateOnly(2024, 1, 1),
                new DateOnly(2024, 12, 31)
            );
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentException when the start date is after the end date.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_StartDateAfterEndDate_ThrowsArgumentException()
        {
            // Arrange
            var timeSlots = new Dictionary<DayOfWeek, (TimeOnly StartTime, TimeOnly EndTime)>();
            timeSlots.Add(DayOfWeek.Monday,
                (new TimeOnly(13, 00),
                 new TimeOnly(14, 00)
                ));

            // Act
            Schedule schedule = new Schedule(
                timeSlots,
                new DateOnly(2024, 12, 31), // Dec 31st, 2024
                new DateOnly(2024, 1, 1) // Jan 1st, 2024
            );
        }

        /// <summary>
        /// Tests that the EnrollStudent method throws an ArgumentNullException when the student parameter is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnrollStudent_NullStudent_ThrowsArgumentNullException()
        {
            // Arrange
            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();
            var rooms = new Dictionary<string, Room>();
            Section section = new Section(
                mockCourse.Object,
                0001,
                mockTeacher.Object,
                mockSchedule.Object,
                rooms);

            // Act
            section.EnrollStudent(null!);
        }

        /// <summary>
        /// Tests that the EnrollStudent method throws an InvalidOperationException when the student is already enrolled.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EnrollStudent_StudentAlreadyEnrolled_ThrowsInvalidOperationException()
        {
            // Arrange
            Mock<Student> mockStudent = new Mock<Student>();

            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();
            var rooms = new Dictionary<string, Room>();
            Section section = new Section(
                mockCourse.Object,
                0001,
                mockTeacher.Object,
                mockSchedule.Object,
                rooms);

            // Act
            section.EnrollStudent(mockStudent.Object);
            section.EnrollStudent(mockStudent.Object);
        }

        /// <summary>
        /// Tests that the EnrollStudent method adds the student to the section's enrolled students list.
        /// </summary>
        // Test originally failed because the Section constructor was given an empty rooms dictionary;
        // The EnrollStudent method internally checks if the section is full by looking at the minimum capacity
        // of the available rooms, so with no rooms this check throws an InvalidOperationException.
        // Fixed this by mocking a Room with a defined NumberOfSeats and including it in the section's room list,
        // allowing the enrollment logic to proceed successfully!
        [TestMethod]
        public void EnrollStudent_ValidStudent_EnrollsStudent()
        {
            //Arrange
            Mock<Student> mockStudent = new Mock<Student>();

            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();

            var mockRoom = new Mock<Room>();
            mockRoom.Setup(r => r.NumberOfSeats).Returns(10); //required for IsSectionFull()

            var rooms = new Dictionary<string, Room>
            {
                { "Classroom", mockRoom.Object }
            };

            Section section = new Section(
                mockCourse.Object,
                1,
                mockTeacher.Object,
                mockSchedule.Object,
                rooms);

            //Act
            section.EnrollStudent(mockStudent.Object);

            //Assert
            Assert.AreEqual(1, section.EnrolledStudents.Count, "Enrolled Students length is not one.");
            Assert.IsTrue(section.EnrolledStudents.Contains(mockStudent.Object), "Student was not enrolled.");
        }

        /// <summary>
        /// Tests that the DropStudent method throws a ArgumentNullException when the student is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DropStudent_StudentIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();
            var rooms = new Dictionary<string, Room>();
            Section section = new Section(
                mockCourse.Object,
                0001,
                mockTeacher.Object,
                mockSchedule.Object,
                rooms);

            // Act
            section.DropStudent(null!);
        }

        /// <summary>
        /// Tests that the DropStudent method removes a student from the section when the student is enrolled.
        /// 
        /// Test failed because the section was constructed without any rooms.
        /// The EnrollStudent method checks if the section is full by inspecting room capacity,
        /// and throws an exception when no rooms are present.
        /// Fixed this by adding a mocked room with a defined NumberOfSeats to the section!
        /// </summary>
        [TestMethod]
        public void DropStudent_StudentIsEnrolled_RemovesStudent()
        {
            //Arrange
            Mock<Student> mockStudent = new Mock<Student>();
            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();

            //Add at least one room so the section can validate capacity
            var mockRoom = new Mock<Room>();
            mockRoom.Setup(r => r.NumberOfSeats).Returns(2);

            var rooms = new Dictionary<string, Room>
            {
                { "Classroom", mockRoom.Object }
            };

            Section section = new Section(
                mockCourse.Object,
                1,
                mockTeacher.Object,
                mockSchedule.Object,
                rooms);

            section.EnrollStudent(mockStudent.Object);

            // Act
            section.DropStudent(mockStudent.Object);

            // Assert
            Assert.AreEqual(0, section.EnrolledStudents.Count,
                            "Enrolled Students length is not zero.");
            Assert.IsFalse(section.EnrolledStudents.Contains(mockStudent.Object),
                            "Student was not dropped.");
        }

        /// <summary>
        /// Tests that the IsStudentEnrolled method returns true when the student is enrolled.
        ///
        /// failed due to an exception in the EnrollStudent method, same fix as previous two!
        /// </summary>
        [TestMethod]
        public void IsStudentEnrolled_StudentIsEnrolled_ReturnsTrue()
        {
            //Arrange
            var mockStudent = new Mock<Student>();
            var mockCourse = new Mock<Course>();
            var mockTeacher = new Mock<Teacher>();
            var mockSchedule = new Mock<Schedule>();

            var mockRoom = new Mock<Room>();
            mockRoom.Setup(r => r.NumberOfSeats).Returns(2); // Make sure section isn't "full"

            var rooms = new Dictionary<string, Room>
            {
                { "RoomA", mockRoom.Object }
            };

            Section section = new Section(
                mockCourse.Object,
                1,
                mockTeacher.Object,
                mockSchedule.Object,
                rooms);

            section.EnrollStudent(mockStudent.Object);

            //Act
            bool result = section.IsStudentEnrolled(mockStudent.Object);

            //Assert
            Assert.IsTrue(result, "Student was not enrolled.");
        }

        /// <summary>
        /// Tests that the IsStudentEnrolled method returns false when the student is not enrolled.
        /// </summary>
        [TestMethod]
        public void IsStudentEnrolled_StudentIsNotEnrolled_ReturnsFalse()
        {
            // Arrange
            Mock<Student> mockStudent = new Mock<Student>();

            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();
            var rooms = new Dictionary<string, Room>();
            Section section = new Section(
                mockCourse.Object,
                0001,
                mockTeacher.Object,
                mockSchedule.Object,
                rooms);

            // Act
            bool result = section.IsStudentEnrolled(mockStudent.Object);

            // Assert
            Assert.IsFalse(result, "Student was enrolled.");
        }

        /// <summary>
        /// Tests that the AddClassSession method throws an ArgumentNullException when the session is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddClassSession_NullSession_ThrowsArgumentNullException()
        {
            // Arrange
            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();
            var rooms = new Dictionary<string, Room>();
            Section section = new Section(
                mockCourse.Object,
                0001,
                mockTeacher.Object,
                mockSchedule.Object,
                rooms);

            // Act
            section.AddClassSession(null!);
        }

        /// <summary>
        /// Tests that the AddClassSession method throws an InvalidOperationException when the session already exists.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddClassSession_SessionAlreadyExists_ThrowsInvalidOperationException()
        {
            // Arrange
            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();
            var rooms = new Dictionary<string, Room>();
            Section section = new Section(
                mockCourse.Object,
                0001,
                mockTeacher.Object,
                mockSchedule.Object,
                rooms);

            Mock<ClassSession> mockSession = new Mock<ClassSession>();

            // Act
            section.AddClassSession(mockSession.Object);
            section.AddClassSession(mockSession.Object);
        }

        /// <summary>
        /// Tests that the AddClassSession method adds a valid session to the section.
        /// </summary>
        [TestMethod]
        public void AddClassSession_ValidSession_AddsSession()
        {
            // Arrange
            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();
            var rooms = new Dictionary<string, Room>();
            Section section = new Section(
                mockCourse.Object,
                0001,
                mockTeacher.Object,
                mockSchedule.Object,
                rooms);

            Mock<ClassSession> mockSession = new Mock<ClassSession>();

            // Act
            section.AddClassSession(mockSession.Object);

            // Assert
            Assert.AreEqual(1, section.ClassSessions.Count, "Session count is not one.");
            Assert.IsTrue(section.ClassSessions.Contains(mockSession.Object), "Session was not added.");
        }

        /// <summary>
        /// Tests that a argument null exception is thrown when removing a null session.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveClassSession_NullSession_ThrowsArgumentNullException()
        {
            // Arrange
            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();
            var rooms = new Dictionary<string, Room>();
            Section section = new Section(
                mockCourse.Object,
                0001,
                mockTeacher.Object,
                mockSchedule.Object,
                rooms);

            // Act
            section.RemoveClassSession(null!);
        }

        /// <summary>
        /// Tests that a session is correctly removed from a section.
        /// </summary>
        [TestMethod]
        public void RemoveClassSession_SessionExists_RemovesSession()
        {
            // Arrange
            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();
            var rooms = new Dictionary<string, Room>();
            Section section = new Section(
                mockCourse.Object,
                0001,
                mockTeacher.Object,
                mockSchedule.Object,
                rooms);

            Mock<ClassSession> mockSession = new Mock<ClassSession>();

            section.AddClassSession(mockSession.Object);

            // Act
            section.RemoveClassSession(mockSession.Object);

            // Assert
            Assert.AreEqual(0, section.ClassSessions.Count, "Session count is not zero.");
            Assert.IsFalse(section.ClassSessions.Contains(mockSession.Object), "Session was not removed.");
        }

        /// <summary>
        /// Tests that the schedule is correctly returned for a course.
        /// </summary>
        [TestMethod]
        public void GetSchedule_ReturnsClassSessions()
        {
            // Arrange
            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();
            var rooms = new Dictionary<string, Room>();
            Section section = new Section(
                mockCourse.Object,
                0001,
                mockTeacher.Object,
                mockSchedule.Object,
                rooms);

            Mock<ClassSession> mockSession = new Mock<ClassSession>();

            section.AddClassSession(mockSession.Object);

            // Act
            HashSet<ClassSession> schedule = section.GetSchedule();

            // Assert
            Assert.AreEqual(1, schedule.Count(), "Schedule count is not one.");
            Assert.IsTrue(schedule.Contains(mockSession.Object), "Session was not included in the schedule.");
        }

        /// <summary>
        /// Tests that the schedule is correctly returned for a course with no class sessions.
        /// </summary>
        [TestMethod]
        public void GetSchedule_ReturnsEmptySet_WhenNoClassSessions()
        {
            // Arrange
            Mock<Course> mockCourse = new Mock<Course>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Schedule> mockSchedule = new Mock<Schedule>();
            var rooms = new Dictionary<string, Room>();
            Section section = new Section(
                mockCourse.Object,
                0001,
                mockTeacher.Object,
                mockSchedule.Object,
                rooms);

            // Act
            HashSet<ClassSession> schedule = section.GetSchedule();

            // Assert
            Assert.AreEqual(0, schedule.Count(), "Schedule count is not zero.");
            Assert.IsFalse(schedule.Contains(null!), "Session was included in the schedule.");
        }

    }
}
