using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Services.ManagerService;
using DawsonDial.Repositories.Interfaces;
using Moq;

namespace DawsonDialTests.ManagerService
{
    /// <summary>
    /// Tests the AdminService class.
    /// </summary>
    [TestClass]
    public class AdminServiceTests
    {
        private Mock<IPersonRepository>? _mockPersonRepository;
        private Mock<IAdminLogRepository>? _mockAdminLogRepository;
        private AdminService? _adminService;

        /// <summary>
        /// Initializes the test class.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _mockPersonRepository = new Mock<IPersonRepository>();
            _mockAdminLogRepository = new Mock<IAdminLogRepository>();
            _adminService = new AdminService(_mockPersonRepository.Object, _mockAdminLogRepository.Object);
        }

        /// <summary>
        /// Tests that the AdminService constructor initializes properties correctly.
        /// </summary>
        [TestMethod]
        public async Task CreateUser_ShouldAddUserAndLogAction()
        {
            // Arrange
            HashSet<Section> emptySections = [];
            var admin = new Admin("admin@dawsoncollege.qc.ca", "password", "Admin", "User", 30, false, "Admin User");
            Student user = new("antoine@dawsoncollege.qc.ca", "password", "Antoine", "Oparin", 19, false, "Chill dude", 2333239, "Computer Science", 2, emptySections);

            // Act
            await _adminService!.CreateUser(admin, user);

            // Assert
            _mockPersonRepository!.Verify(repo => repo.AddAsync(user), Times.Once);
            _mockAdminLogRepository!.Verify(repo => repo.AddAsync(It.IsAny<AdminLog>()), Times.Once);
        }

        /// <summary>
        /// Tests that the UpdateLastLogin method updates the last login timestamp.
        /// </summary>
        [TestMethod]
        public async Task UpdateLastLogin_ShouldUpdateLastLogin()
        {
            // Arrange
            var admin = new Admin("admin@dawsoncollege.qc.ca", "password", "Admin", "User", 30, false, "Admin User");

            // Act
            await _adminService!.UpdateLastLogin(admin);

            // Assert
            Assert.IsNotNull(admin.LastLogin);
        }

        /// <summary>
        /// Tests that the DisableUser method disables the user and logs the action.
        /// </summary>
        [TestMethod]
        public async Task DisableUser_ShouldDisableUserAndLogAction()
        {
            // Arrange
            HashSet<Section> emptySections = [];
            var admin = new Admin("admin@dawsoncollege.qc.ca", "password", "Admin", "User", 30, false, "Admin User");
            Student user = new("antoine@dawsoncollege.qc.ca", "password", "Antoine", "Oparin", 19, false, "Chill dude", 2333239, "Computer Science", 2, emptySections);

            // Act
            await _adminService!.DisableUser(admin, user);

            // Assert
            Assert.IsTrue(user.IsDisabled);
            _mockAdminLogRepository!.Verify(repo => repo.AddAsync(It.IsAny<AdminLog>()), Times.Once);
        }

        /// <summary>
        /// Tests that the EnableUser method enables the user and logs the action.
        /// </summary>
        [TestMethod]
        public async Task EnableUser_ShouldEnableUserAndLogAction()
        {
            // Arrange
            HashSet<Section> emptySections = [];
            var admin = new Admin("admin@dawsoncollege.qc.ca", "password", "Admin", "User", 30, false, "Admin User");
            Student user = new("antoine@dawsoncollege.qc.ca", "password", "Antoine", "Oparin", 19, false, "Chill dude", 2333239, "Computer Science", 2, emptySections);

            // Act
            await _adminService!.EnableUser(admin, user);

            // Assert
            Assert.IsFalse(user.IsDisabled);
            _mockAdminLogRepository!.Verify(repo => repo.AddAsync(It.IsAny<AdminLog>()), Times.Once);
        }

        /// <summary>
        /// Tests that the RemoveUser method removes the user and logs the action.
        /// </summary>
        [TestMethod]
        public async Task RemoveUser_ShouldRemoveUserAndLogAction()
        {
            // Arrange
            HashSet<Section> emptySections = [];
            var admin = new Admin("admin@dawsoncollege.qc.ca", "password", "Admin", "User", 30, false, "Admin User");
            Student user = new("antoine@dawsoncollege.qc.ca", "password", "Antoine", "Oparin", 19, false, "Chill dude", 2333239, "Computer Science", 2, emptySections);

            _mockPersonRepository!.Setup(repo => repo.DeleteAsync(user.PersonId)).Returns(Task.CompletedTask);
            _mockAdminLogRepository!.Setup(repo => repo.AddAsync(It.IsAny<AdminLog>())).Returns(Task.CompletedTask);

            // Act
            await _adminService!.RemoveUser(admin, user);

            // Assert
            _mockPersonRepository.Verify(repo => repo.DeleteAsync(user.PersonId), Times.Once);
            _mockAdminLogRepository.Verify(repo => repo.AddAsync(It.IsAny<AdminLog>()), Times.Once);
        }

        /// <summary>
        /// Tests that the GetLogsForAdmin method returns logs for the specified admin.
        /// </summary>
        [TestMethod]
        public async Task GetLogsForAdmin_ShouldReturnLogsForAdmin()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var logs = new List<AdminLog>
            {
                new AdminLog { AdminId = adminId, Action = "Action 1", Timestamp = DateTime.UtcNow },
                new AdminLog { AdminId = Guid.NewGuid(), Action = "Action 2", Timestamp = DateTime.UtcNow }
            };

            _mockAdminLogRepository!.Setup(repo => repo.GetAllAsync()).ReturnsAsync(logs);

            // Act
            var result = await _adminService!.GetLogsForAdmin(adminId);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Action 1", result.First().Action);
        }
    }
}