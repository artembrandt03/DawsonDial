using DawsonDial.Models.People;

namespace DawsonDialTests
{
    /// <summary>
    /// Tests the Admin class.
    /// </summary>
    [TestClass]
    public class AdminTests
    {
        /// <summary>
        /// Helper method to create a new admin user for testing.
        /// </summary>
        private Admin CreateAdminUser()
        {
            return new Admin("adminman@dawsoncollege.qc.ca", "password123", "Admin", "Man", 19, false, "Admin dude");
        }

        /// <summary>
        /// Tests that the Admin constructor initializes properties correctly.
        /// </summary>
        [TestMethod]
        public void LastLogin_ShouldNotBeNull()
        {
            // Arrange
            var admin = CreateAdminUser();
            var expectedLoginTime = DateTime.Now;

            // Act
            admin.LastLogin = expectedLoginTime;

            // Assert
            Assert.IsNotNull(admin.LastLogin);
            Assert.AreEqual(expectedLoginTime, admin.LastLogin);
        }

        /// <summary>
        /// Tests that the CreatedUsersCount property is initialized to 0.
        /// </summary>
        [TestMethod]
        public void LogAction_ShouldAddLogEntry()
        {
            // Arrange
            var admin = CreateAdminUser();
            var action = "Created a new event";

            // Act
            admin.LogAction(action);

            // Assert
            Assert.AreEqual(1, admin.Logs.Count);
            Assert.AreEqual(action, admin.Logs.First().Action);
        }

        /// <summary>
        /// Tests that the CreatedUsersCount property is incremented correctly.
        /// </summary>
        [TestMethod]
        public void LogAction_ShouldAddLogEntryWithCorrectTimestamp()
        {
            // Arrange
            var admin = CreateAdminUser();
            var action = "Created a new event";
            var expectedTimestamp = DateTime.Now;

            // Act
            admin.LogAction(action);

            // Assert
            Assert.AreEqual(1, admin.Logs.Count);
            Assert.AreEqual(action, admin.Logs.First().Action);
            Assert.IsTrue(admin.Logs.First().Timestamp >= expectedTimestamp);
        }

        /// <summary>
        /// Tests that the CreatedUsersCount property is incremented correctly.
        /// </summary>
        [TestMethod]
        public void LogAction_ShouldAddLogEntryWithCorrectAdminId()
        {
            // Arrange
            var admin = CreateAdminUser();
            var action = "Created a new event";

            // Act
            admin.LogAction(action);

            // Assert
            Assert.AreEqual(1, admin.Logs.Count);
            Assert.AreEqual(admin.PersonId, admin.Logs.First().AdminId);
        }

        /// <summary>
        /// Tests that the CreatedUsersCount property is incremented correctly.
        /// </summary>
        [TestMethod]
        public void IncrementUserCreationCount_ShouldIncrementCount()
        {
            // Arrange
            var admin = CreateAdminUser();
            var initialCount = admin.CreatedUsersCount;

            // Act
            admin.IncrementUserCreationCount();

            // Assert
            Assert.AreEqual(initialCount + 1, admin.CreatedUsersCount);
        }

        /// <summary>
        /// Tests that the LastLogin property is updated correctly.
        /// </summary>
        [TestMethod]
        public void UpdateLastLogin_ShouldUpdateLastLoginTime()
        {
            // Arrange
            var admin = CreateAdminUser();
            var initialLoginTime = admin.LastLogin;

            // Add a small delay to ensure the timestamps will be different
            System.Threading.Thread.Sleep(10);

            // Act
            admin.UpdateLastLogin();

            // Assert
            Assert.AreNotEqual(initialLoginTime, admin.LastLogin, "LastLogin should be updated to a different time.");
        }
    }
}