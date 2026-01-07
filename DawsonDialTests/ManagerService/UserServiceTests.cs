//TEST DRIVEN DEVELOPMENT

//From Lucidchart:
// UserService
// Attrributes:
//  - _users : HashSet<Person>
// Methods:
//  + Authenticate(string, string) : bool
//  + GetUserByUsername(string) : Person
//  + UpdateUser(Person) : void
//  + ChangePassword(string, string, string) : void
// ADDITIONAL METHODS I THOUGHT OF:
// + AddUser(Person user) : void
// + GetAllUsers() : HashSet<Person>


using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using DawsonDial.Repositories.Interfaces;
using Moq;

namespace DawsonDialTests.ManagerService
{
    /// <summary>
    /// Tests the UserService class.
    /// </summary>
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IPersonRepository> mockRepo = null!;
        private UserService service = null!;
        private List<Person> store = null!;

        /// <summary>
        /// Sets up the test environment by initializing the mock repository and service.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            mockRepo = new Mock<IPersonRepository>();
            service = new UserService(mockRepo.Object);
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
        /// Tests that AddUser adds a user to the collection.
        /// </summary>
        [TestMethod]
        public async Task AddUser_ShouldAddUserToCollection()
        {
            // Arrange
            var user = new Mock<Person>();
            user.Setup(u => u.Username).Returns("a@dawsoncollege.qc.ca");
            user.SetupProperty(u => u.Password, "12345678");
            // Act
            await service.AddUser(user.Object);
            var result = await service.GetAllUsers();

            // Assert
            Assert.AreEqual(1, result.Count());
        }

        /// <summary>
        /// Tests that Authenticate returns true for valid credentials.
        /// </summary>
        [TestMethod]
        public async Task Authenticate_ShouldReturnTrue_WhenCredentialsMatch()
        {
            // Arrange
            var user = new Mock<Person>();
            user.Setup(u => u.Username).Returns("b@dawsoncollege.qc.ca");
            user.SetupProperty(u => u.Password, "12345678");
            store.Add(user.Object);

            // Act
            var result = await service.Authenticate("b@dawsoncollege.qc.ca", "12345678");

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Tests that Authenticate returns false for incorrect password.
        /// </summary>
        [TestMethod]
        public async Task Authenticate_ShouldReturnFalse_WhenCredentialsIncorrect()
        {
            // Arrange
            var user = new Mock<Person>();
            user.Setup(u => u.Username).Returns("c@dawsoncollege.qc.ca");
            user.SetupProperty(u => u.Password, "12345678");
            store.Add(user.Object);

            // Act
            var result = await service.Authenticate("c@dawsoncollege.qc.ca", "wrongpass");

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests that Authenticate returns false when user is not found.
        /// </summary>
        [TestMethod]
        public async Task Authenticate_ShouldReturnFalse_WhenUserNotFound()
        {
            // Act
            var result = await service.Authenticate("ghost@dawsoncollege.qc.ca", "12345678");

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests retrieving a user by username.
        /// </summary>
        [TestMethod]
        public async Task GetUserByUsername_ShouldReturnCorrectUser()
        {
            // Arrange
            var user = new Mock<Person>();
            user.Setup(u => u.Username).Returns("d@dawsoncollege.qc.ca");
            store.Add(user.Object);

            // Act
            var found = await service.GetUserByUsername("d@dawsoncollege.qc.ca");

            // Assert
            Assert.IsNotNull(found);
            Assert.AreEqual("d@dawsoncollege.qc.ca", found.Username);
        }

        /// <summary>
        /// Tests retrieving all registered users.
        /// </summary>
        [TestMethod]
        public async Task GetAllUsers_ShouldReturnAll()
        {
            // Arrange
            var user1 = new Mock<Person>();
            user1.Setup(u => u.Username).Returns("e1@dawsoncollege.qc.ca");
            var user2 = new Mock<Person>();
            user2.Setup(u => u.Username).Returns("e2@dawsoncollege.qc.ca");
            store.Add(user1.Object);
            store.Add(user2.Object);

            // Act
            var all = await service.GetAllUsers();

            // Assert
            Assert.AreEqual(2, all.Count());
        }

        /// <summary>
        /// Tests that UpdateUser updates the user details.
        /// </summary>
        [TestMethod]
        public async Task UpdateUser_ShouldUpdateDetails()
        {

            // Arrange
            var original = new Mock<Person>();
            original.Setup(u => u.Username).Returns("f@dawsoncollege.qc.ca");
            original.SetupProperty(u => u.Description, "Old");
            store.Add(original.Object);

            var updated = new Mock<Person>();
            updated.Setup(u => u.Username).Returns("f@dawsoncollege.qc.ca");
            updated.Setup(u => u.Description).Returns("New");

            // Act
            await service.UpdateUser(updated.Object);
            var found = await service.GetUserByUsername("f@dawsoncollege.qc.ca");

            // Assert
            Assert.AreEqual("New", found.Description);
        }

        /// <summary>
        /// Tests that UpdatePassword updates the password.
        /// </summary>
        [TestMethod]
        public async Task UpdatePassword_ShouldUpdatePassword()
        {
            // Arrange
            var user = new Mock<Person>();
            user.Setup(u => u.Username).Returns("g@dawsoncollege.qc.ca");
            user.SetupProperty(u => u.Password, "oldpass");
            store.Add(user.Object);

            // Act
            await service.UpdatePassword("g@dawsoncollege.qc.ca", "oldpass", "newpass");

            // Assert
            Assert.AreEqual("newpass", user.Object.Password);
        }

        /// <summary>
        /// Tests that UpdatePassword throws when the old password is incorrect.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public async Task UpdatePassword_ShouldThrow_WhenOldPasswordIncorrect()
        {
            // Arrange
            var user = new Mock<Person>();
            user.Setup(u => u.Username).Returns("h@dawsoncollege.qc.ca");
            user.SetupProperty(u => u.Password, "correct");
            store.Add(user.Object);

            // Act & Assert
            await service.UpdatePassword("h@dawsoncollege.qc.ca", "wrong", "newpass");
        }

        /// <summary>
        /// Tests that AddUser throws an ArgumentNullException when the input is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task AddUser_ShouldThrow_WhenUserNull()
        {
            // Act & Assert
            await service.AddUser(null!);
        }
    }
}
