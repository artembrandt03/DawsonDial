using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;
using DawsonDial.Services.ManagerService;
using DawsonDial.Repositories.Interfaces;
using Moq;

//Beginning to fix unit tests
namespace DawsonDialTests
{
    /// <summary>
    /// Tests the CourseService class.
    /// </summary>
    [TestClass]
    public class CourseServiceTests
    {
        /// <summary>
        /// Tests that AddCourse adds a course successfully.
        /// </summary>
        [TestMethod]
        public async Task AddCourse_AddsSuccessfully()
        {
            // Arrange
            var mockCourseRepo = new Mock<ICourseRepository>();
            var mockSectionRepo = new Mock<ISectionRepository>();
            var courseService = new CourseService(mockCourseRepo.Object, mockSectionRepo.Object);

            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Section> mockSection = new Mock<Section>();
            var teachers = new HashSet<Teacher> { mockTeacher.Object };
            var sections = new HashSet<Section> { mockSection.Object };
            Course course = new Course("420-401", "Programming", teachers, sections);

            // Act
            await courseService.AddCourse(course);

            // Assert
            mockCourseRepo.Verify(r => r.AddAsync(course), Times.Once);
        }

        /// <summary>
        /// Tests that RemoveCourse removes a course successfully.
        /// </summary>
        [TestMethod]
        public async Task RemoveCourse_RemovesSuccessfully()
        {
            // Arrange
            var mockCourseRepo = new Mock<ICourseRepository>();
            var mockSectionRepo = new Mock<ISectionRepository>();
            var courseService = new CourseService(mockCourseRepo.Object, mockSectionRepo.Object);
            Mock<Section> mockSection = new Mock<Section>();
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            var teachers = new HashSet<Teacher> { mockTeacher.Object };
            var sections = new HashSet<Section> { mockSection.Object };
            Course course = new Course("420-401", "Programming", teachers, sections);

            // Act
            await courseService.RemoveCourse(course);

            // Assert
            mockCourseRepo.Verify(r => r.DeleteAsync(course.CourseId), Times.Once);

        }

        /// <summary>
        /// Tests the UpdateCourse method to ensure it updates a course successfully.
        /// </summary>
        [TestMethod]
        public async Task UpdateCourse_UpdatesCourseSuccessfully()
        {
            // Arrange
            var mockCourseRepo = new Mock<ICourseRepository>();
            var mockSectionRepo = new Mock<ISectionRepository>();
            var courseService = new CourseService(mockCourseRepo.Object, mockSectionRepo.Object);

            // Updated course
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Section> mockSection = new Mock<Section>();
            var teachers = new HashSet<Teacher> { mockTeacher.Object };
            var sections = new HashSet<Section> { mockSection.Object };
            Mock<Section> updatedSection = new Mock<Section>();
            var updatedSections = new HashSet<Section> { updatedSection.Object };
            Course updatedCourse = new Course("420-402", "Programming II", teachers, updatedSections);

            // Act
            await courseService.UpdateCourse(updatedCourse);

            // Assert
            mockCourseRepo.Verify(r => r.UpdateAsync(updatedCourse), Times.Once);
        }
        /// <summary>
        /// Tests that AddSection adds a section successfully.
        /// </summary>
        /// <returns></returns>
        /// FIX:
        /// There was an issue caused by mocking: 
        /// the Section class determines if it's full based on the room with the smallest NumberOfSeats, but our mocked room didn't have that property configured! 
        /// So it defaulted to zero—making the section appear full and throwing an exception. 
        /// Resolved this by explicitly setting NumberOfSeats to a hard coded value (10 in this case) on the mocked room 
        /// using mockRoom.Setup(r => r.NumberOfSeats).Returns(10);. 
        /// This allowed the enrollment to proceed as expected!
        [TestMethod]
        public async Task EnrollStudent_EnrollsStudentSuccessfully()
        {
            //Arrange
            var mockCourseRepo = new Mock<ICourseRepository>();
            var mockSectionRepo = new Mock<ISectionRepository>();
            var courseService = new CourseService(mockCourseRepo.Object, mockSectionRepo.Object);

            var teacher = new Mock<Teacher>().Object;
            var schedule = new Mock<Schedule>().Object;

            //Set the NumberOfSeats so IsSectionFull() doesn't throw an exception
            var mockRoom = new Mock<Room>();
            mockRoom.Setup(r => r.NumberOfSeats).Returns(10);

            var rooms = new Dictionary<string, Room> { { "Classroom", mockRoom.Object } };

            var course = new Course("420-401", "Programming", new HashSet<Teacher>(), new HashSet<Section>());
            var section = new Section(course, 1, teacher, schedule, rooms);

            var student = new Mock<Student>().Object;

            //ensures the section exists in the repo (avoids ArgumentException)
            mockSectionRepo.Setup(r => r.GetByIdAsync(section.SectionId)).ReturnsAsync(section);

            //Act
            await courseService.EnrollStudent(section, student);

            //Assert
            mockSectionRepo.Verify(r => r.UpdateAsync(section), Times.Once);
            Assert.IsTrue(section.EnrolledStudents.Contains(student), "Student was not enrolled successfully.");
        }

        /// <summary>
        /// Tests that UnenrollStudent removes a student from a course successfully.
        /// </summary>
        /// FIX:
        /// There was an issue caused by the UpdateAsync method being called twice during the EnrollStudent setup 
        /// and again during the actual UnenrollStudent method under test! 
        /// The original Verify assertion expected only one call, which led to a test failure. 
        /// The way I fixed this is I added a call to mockSectionRepo.Invocations.Clear() after enrolling the student, 
        /// basically resetting the mock's call history. 
        /// This ensured that the Verify statement only validated the UpdateAsync call triggered by the unenrollment. 
        /// This fix is similar to EnrollStudent test, which required mocking the repository’s GetByIdAsync method to simulate an existing section. 
        /// The difference here was the need to isolate the mock’s interaction history between two method calls in the same test!
        [TestMethod]
        public async Task UnenrollStudent_UnenrollsStudentSuccessfully()
        {
            //Arrange
            var mockCourseRepo = new Mock<ICourseRepository>();
            var mockSectionRepo = new Mock<ISectionRepository>();
            var courseService = new CourseService(mockCourseRepo.Object, mockSectionRepo.Object);

            var mockTeacher = new Mock<Teacher>();
            var mockSchedule = new Mock<Schedule>();
            var mockRoom = new Mock<Room>();
            mockRoom.Setup(r => r.NumberOfSeats).Returns(10);

            var rooms = new Dictionary<string, Room> { { "Classroom", mockRoom.Object } };
            var course = new Course("420-401", "Programming", new HashSet<Teacher> { mockTeacher.Object }, new HashSet<Section>());

            var section = new Section(course, 1, mockTeacher.Object, mockSchedule.Object, rooms);
            var mockStudent = new Mock<Student>();

            //Setup for both calls (Enroll + Unenroll)
            mockSectionRepo.Setup(r => r.GetByIdAsync(section.SectionId)).ReturnsAsync(section);

            //First enroll the student
            await courseService.EnrollStudent(section, mockStudent.Object);

            //Clear invocation history to isolate next call
            mockSectionRepo.Invocations.Clear();

            //Act - Unenroll student
            await courseService.UnenrollStudent(section, mockStudent.Object);

            //Assert
            mockSectionRepo.Verify(r => r.UpdateAsync(section), Times.Once);
            Assert.IsFalse(section.EnrolledStudents.Contains(mockStudent.Object), "Student was not unenrolled successfully.");
        }

        /// <summary>
        /// Tests that GetAllCourses returns all courses.
        /// </summary>
        [TestMethod]
        public async Task GetAllCourses_ReturnsAllCourses()
        {
            // Arrange
            var mockCourseRepo = new Mock<ICourseRepository>();
            var mockSectionRepo = new Mock<ISectionRepository>();
            var courseService = new CourseService(mockCourseRepo.Object, mockSectionRepo.Object);

            // Course 1
            Mock<Teacher> mockTeacher = new Mock<Teacher>();
            Mock<Section> mockSection = new Mock<Section>();
            var teachers = new HashSet<Teacher> { mockTeacher.Object };
            var sections = new HashSet<Section> { mockSection.Object };
            Course course1 = new Course("420-401", "Programming", teachers, sections);

            // Course 2
            Mock<Teacher> mockTeacher2 = new Mock<Teacher>();
            Mock<Section> mockSection2 = new Mock<Section>();
            var teachers2 = new HashSet<Teacher> { mockTeacher2.Object };
            var sections2 = new HashSet<Section> { mockSection2.Object };
            Course course2 = new Course("420-402", "Data Structures", teachers2, sections2);

            mockCourseRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new HashSet<Course> { course1, course2 });

            // Act
            var courses = await courseService.GetAllCourses();

            // Assert
            Assert.AreEqual(2, courses.Count(), "Number of courses is not correct.");
            Assert.IsTrue(courses.Contains(course1), "Course 1 was not returned.");
            Assert.IsTrue(courses.Contains(course2), "Course 2 was not returned.");
        }
    }
}
