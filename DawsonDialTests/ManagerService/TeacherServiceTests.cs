using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Models.Rooms;
using DawsonDial.Services.ManagerService;
using DawsonDial.Repositories.Interfaces;
using Moq;

namespace DawsonDialTests.ManagerService
{
    /// <summary>
    /// Tests the TeacherService class.
    /// </summary>
    [TestClass]
    public class TeacherServiceTests
    {
        /// <summary>
        /// Creates a test teacher.
        /// </summary>
        /// <returns>A new instance of the Teacher class.</returns>
        private Teacher CreateTestTeacher()
        {
            Mock<Office> mockOffice = new();
            HashSet<Section> emptySections = new();
            Mock<OfficeHours> mockOfficeHours = new();

            // Act
            return new Teacher(
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
        }

        /// <summary>
        /// Tests that AddCourse adds a new section to the teacher's list of classes.
        /// </summary>
        [TestMethod]
        public async Task AddCourse_ShouldAddNewSection()
        {
            // Arrange
            var mockRepo = new Mock<IPersonRepository>();
            var teacherService = new TeacherService(mockRepo.Object);
            var teacher = CreateTestTeacher();
            var section = new Mock<Section>();

            // Act
            await teacherService.AddCourse(teacher, section.Object);

            // Assert
            Assert.IsTrue(teacher.Classes.Contains(section.Object));
        }

        /// <summary>
        /// Tests that AddCourse does not add duplicate sections.
        /// </summary>
        [TestMethod]
        public async Task AddCourse_ShouldNotAddDuplicateSection()
        {
            // Arrange
            var mockRepo = new Mock<IPersonRepository>();
            var teacherService = new TeacherService(mockRepo.Object);
            var teacher = CreateTestTeacher();
            var section1 = new Mock<Section>();
            var section2 = section1;

            // Act
            await teacherService.AddCourse(teacher, section1.Object);
            await teacherService.AddCourse(teacher, section2.Object);

            // Assert: Only one section should be present.
            Assert.AreEqual(1, teacher.Classes.Count);
        }

        /// <summary>
        /// Tests that SetOfficeHour adds a schedule.
        /// </summary>
        [TestMethod]
        public async Task SetOfficeHour_ShouldAddSchedule()
        {
            // Arrange
            var mockRepo = new Mock<IPersonRepository>();
            var teacherService = new TeacherService(mockRepo.Object);
            var teacher = CreateTestTeacher();
            var officeHours = new Mock<OfficeHours>();

            // Setup the repository to update the teacher's office hours
            mockRepo.Setup(r => r.SetOfficeHoursAsync(It.IsAny<Teacher>(), It.IsAny<OfficeHours>()))
                .Callback<Teacher, OfficeHours>((t, oh) => t.OfficeHours = oh)
                .Returns(Task.CompletedTask);

            // Act
            await teacherService.SetOfficeHoursAsync(teacher, officeHours.Object);

            // Assert
            Assert.AreEqual(officeHours.Object, teacher.OfficeHours);
        }

        /// <summary>
        /// Tests that GetOfficeHours returns all the teacher's office hours.
        /// </summary>
        [TestMethod]
        public async Task GetOfficeHours_ShouldReturnAllSchedules()
        {
            // Arrange
            var mockRepo = new Mock<IPersonRepository>();
            var teacherService = new TeacherService(mockRepo.Object);
            var teacher = CreateTestTeacher();
            var officeHours = new Mock<OfficeHours>();

            // Setup the repository to update the teacher's office hours
            mockRepo.Setup(r => r.SetOfficeHoursAsync(It.IsAny<Teacher>(), It.IsAny<OfficeHours>()))
                .Callback<Teacher, OfficeHours>((t, oh) => t.OfficeHours = oh)
                .Returns(Task.CompletedTask);

            await teacherService.SetOfficeHoursAsync(teacher, officeHours.Object);

            // Act
            var officeHoursGotten = teacherService.GetOfficeHours(teacher);

            // Assert
            Assert.AreEqual(officeHours.Object, officeHoursGotten);
        }

        /// <summary>
        /// Tests that DeleteCourseSection removes an existing section.
        /// </summary>
        [TestMethod]
        public async Task DeleteCourseSection_ShouldRemoveSection()
        {
            // Arrange
            var mockRepo = new Mock<IPersonRepository>();
            var teacherService = new TeacherService(mockRepo.Object);
            var teacher = CreateTestTeacher();
            var section = new Mock<Section>();

            await teacherService.AddCourse(teacher, section.Object);

            // Act
            await teacherService.DeleteCourseSection(teacher, section.Object);

            // Assert
            Assert.IsFalse(teacher.Classes.Contains(section.Object));
        }

        /// <summary>
        /// Tests that DeleteCourseSection throws an exception when the section does not exist.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task DeleteCourseSection_ShouldThrowException_WhenSectionNotFound()
        {
            // Arrange
            var mockRepo = new Mock<IPersonRepository>();
            var teacherService = new TeacherService(mockRepo.Object);
            var teacher = CreateTestTeacher();
            Section section = new Section(new Mock<Course>().Object, 23, teacher, new Mock<Schedule>().Object, new Dictionary<string, Room>());

            // Act & Assert
            await teacherService.DeleteCourseSection(teacher, section);
        }
    }
}
