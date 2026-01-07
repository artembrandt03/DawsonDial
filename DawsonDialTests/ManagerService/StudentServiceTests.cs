
using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Models.Rooms;
using DawsonDial.Services.ManagerService;
using DawsonDial.Repositories.Interfaces;
using Moq;

namespace DawsonDialTests.ManagerService
{
    /// <summary>
    /// Tests the StudentService class.
    /// </summary>
    [TestClass]
    public class StudentServiceTests
    {
        private Mock<IPersonRepository> mockRepo = null!;
        private StudentService service = null!;
        private List<Person> store = null!;

        /// <summary>
        /// Sets up the test environment by initializing the mock repository and service.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            mockRepo = new Mock<IPersonRepository>();
            service = new StudentService(mockRepo.Object);
            store = new List<Person>();

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Person>())).Callback<Person>(p => store.Add(p)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(() => store);
            mockRepo.Setup(r => r.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync((string username) => store.FirstOrDefault(u => u.Username == username)!);

            mockRepo.Setup(r => r.GetUserByUsernameAndPasswordAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string username, string password) => {
                var user = store.FirstOrDefault(u => u.Username == username);
                return user != null && user.Password == password ? user : null!;
            });

            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Person>())).Callback<Person>(p => {
                var index = store.FindIndex(u => u.Username == p.Username);
                if (index >= 0) store[index] = p;
            }).Returns(Task.CompletedTask);

            mockRepo.Setup(r => r.UpdateUserPasswordByUsernameAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string username, string newPass) => {
                var person = store.First(u => u.Username == username);
                person.Password = newPass;
                return person;
            });
        }

        /// <summary>
        /// Tests enrolling a student in a course section.
        /// </summary>
        [TestMethod]
        public async Task EnrollInCourse_ShouldAddSectionToStudent()
        {
            var section = new Mock<Section>();
            section.Setup(s => s.SectionId).Returns(Guid.NewGuid());
            section.Setup(s => s.EnrolledStudents).Returns(new HashSet<Student>());

            var student = new Student("s@dawsoncollege.qc.ca", "password123", "Stu", "Dent", 20, false, "", 1234567, "Science", 1, new());

            await service.EnrollInCourse(student, section.Object);
            Assert.IsTrue(student.Classes.Contains(section.Object));
        }

        /// <summary>
        /// Tests dropping a student from a course section.
        /// </summary>
        [TestMethod]
        public async Task DropCourse_ShouldRemoveSectionFromStudent()
        {
            var section = new Mock<Section>();
            section.Setup(s => s.SectionId).Returns(Guid.NewGuid());
            section.Setup(s => s.EnrolledStudents).Returns(new HashSet<Student>());

            var student = new Student("s2@dawsoncollege.qc.ca", "password123", "Stu", "Dent", 20, false, "", 1234568, "Arts", 2, new());
            student.AddClass(section.Object);
            Assert.IsTrue(student.Classes.Contains(section.Object));

            await service.DropCourse(student, section.Object);
            Assert.IsFalse(student.Classes.Contains(section.Object));
        }
    }
}