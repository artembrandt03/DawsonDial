// Test_NumberOfSeats_ShouldBePositive: Seats count should be > 0
// Test_IsAvailable_ShouldBeCorrectlyTracked: Availablitly should be correct
using DawsonDial.Models.Rooms;

namespace DawsonDialTests.Models.Rooms
{
    [TestClass]
    public class IRoomTests
    {
        //1: Testing so that the number of seats is always positive (Talon's suggested test)
        [TestMethod]
        public void Test_NumberOfSeats_ShouldBePositive()
        {
            //Arrange
            var room = new Office("2F.24", 10, false);

            //Act & Assert
            Assert.IsTrue(room.NumberOfSeats > 0, "num of seats should be positive");
        }

        //2: Testing that a new room is available by default
        [TestMethod]
        public void Test_NewRoom_ShouldBeAvailableByDefault()
        {
            //Arrange
            var room = new Office("2F.24", 10, false);

            //Act & Assert
            Assert.IsTrue(room.IsAvailable, "a new room should be available by default");
        }

        //3: Testing reserving a room which then sets it to unavailable
        [TestMethod]
        public void Test_ReserveRoom_ShouldMakeUnavailable()
        {
            //Arrange
            var room = new Office("2F.24", 10, false);

            //Act
            room.ReserveRoom();

            //Assert
            Assert.IsFalse(room.IsAvailable, "reserving a room should make it unavailable to reserve again");
        }

        //4: Testing releasing a room which then makes it available
        [TestMethod]
        public void Test_ReleaseRoom_ShouldMakeAvailable()
        {
            //Arrange
            var room = new Office("2F.24", 10, false);
            room.ReserveRoom();

            //Act
            room.ReleaseRoom();

            //Assert
            Assert.IsTrue(room.IsAvailable, "releasing a room should make it available");
        }

        //5: Final test - isAvailable should be correectly tracked (talon's suggestion)
        [TestMethod]
        public void Test_IsAvailable_ShouldBeCorrectlyTracked()
        {
            //Arrange
            var room = new Office("4F.04", 10, false);
            
            //Act
            bool initialAvailability = room.IsAvailable; //saving into a bool
            room.ReserveRoom();
            bool afterReserve = room.IsAvailable;
            room.ReleaseRoom();
            bool afterRelease = room.IsAvailable;

            // Assert
            Assert.IsTrue(initialAvailability, "room should be available initially");
            Assert.IsFalse(afterReserve, "room should NOT be unavailable after reservation");
            Assert.IsTrue(afterRelease, "room SHOULD be available again after release");
        }
    }
}