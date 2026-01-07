using DawsonDial.Models.Events;
using DawsonDial.Services.ManagerService;
using DawsonDial.Repositories.Interfaces;
using Moq;

namespace DawsonDialTests
{
    /// <summary>
    /// Tests for the ScheduleService class.
    /// </summary>
    [TestClass]
    public class ScheduleServiceTests
    {
        /// <summary>
        /// Tests the AddSchedule method adds a schedule to the schedule service.
        /// </summary>
        [TestMethod]
        public async Task AddSchedule_ValidSchedule_ShouldAddSchedule()
        {
            // Arrange
            var mockRepo = new Mock<IScheduleRepository>();
            ScheduleService scheduleService = new ScheduleService(mockRepo.Object);
            var timeSlots = new Dictionary<DayOfWeek, (TimeOnly StartTime, TimeOnly EndTime)>
            {
                { DayOfWeek.Monday, (new TimeOnly(9, 0), new TimeOnly(10, 30)) },
                { DayOfWeek.Wednesday, (new TimeOnly(13, 0), new TimeOnly(14, 30)) }
            };

            DateOnly startDate = new DateOnly(2024, 1, 1);
            DateOnly endDate = new DateOnly(2024, 4, 30);

            Schedule testSchedule = new Schedule(timeSlots, startDate, endDate);

            // Act
            await scheduleService.AddSchedule(testSchedule);

            // Assert
            mockRepo.Verify(r => r.AddAsync(testSchedule), Times.Once);
        }

        /// <summary>
        /// Tests the UpdateSchedule method updates a schedule in the schedule service.
        /// </summary>
        [TestMethod]
        public async Task UpdateSchedule_ShouldUpdateSchedule()
        {
            // Arrange
            var mockRepo = new Mock<IScheduleRepository>();
            ScheduleService scheduleService = new ScheduleService(mockRepo.Object);

            // Updated schedule
            var updatedTimeSlots = new Dictionary<DayOfWeek, (TimeOnly StartTime, TimeOnly EndTime)>
            {
                { DayOfWeek.Monday, (new TimeOnly(10, 0), new TimeOnly(11, 30)) },
                { DayOfWeek.Wednesday, (new TimeOnly(14, 0), new TimeOnly(15, 30)) }
            };

            DateOnly updatedStartDate = new DateOnly(2024, 2, 1);
            DateOnly updatedEndDate = new DateOnly(2024, 5, 30);

            Schedule updatedSchedule = new Schedule(updatedTimeSlots, updatedStartDate, updatedEndDate);

            // Act
            await scheduleService.UpdateSchedule(updatedSchedule);

            // Assert
            mockRepo.Verify(r => r.UpdateAsync(updatedSchedule), Times.Once);
        }

        /// <summary>
        /// Tests that the RemoveSchedule method removes a schedule from the schedule service.
        /// </summary>
        [TestMethod]
        public async Task RemoveSchedule_ShouldRemoveSchedule()
        {
            // Arrange
            var mockRepo = new Mock<IScheduleRepository>();
            ScheduleService scheduleService = new ScheduleService(mockRepo.Object);

            var timeSlots = new Dictionary<DayOfWeek, (TimeOnly StartTime, TimeOnly EndTime)>
            {
                { DayOfWeek.Monday, (new TimeOnly(9, 0), new TimeOnly(10, 30)) },
                { DayOfWeek.Wednesday, (new TimeOnly(13, 0), new TimeOnly(14, 30)) }
            };

            DateOnly startDate = new DateOnly(2024, 1, 1);
            DateOnly endDate = new DateOnly(2024, 4, 30);
            Schedule testSchedule = new Schedule(timeSlots, startDate, endDate); //declaring the actual testSchedule

            // Act
            await scheduleService.RemoveSchedule(testSchedule);

            // Assert
            mockRepo.Verify(r => r.DeleteAsync(testSchedule.ScheduleId), Times.Once);
        }

        /// <summary>
        /// Tests that GetAllSchedules returns all schedules.
        /// </summary>
        [TestMethod]
        public async Task GetAllSchedules_ShouldReturnAllSchedules()
        {
            // Arrange
            var mockRepo = new Mock<IScheduleRepository>();
            ScheduleService scheduleService = new ScheduleService(mockRepo.Object);

            // First Schedule
            var timeSlots = new Dictionary<DayOfWeek, (TimeOnly StartTime, TimeOnly EndTime)>
            {
                { DayOfWeek.Monday, (new TimeOnly(9, 0), new TimeOnly(10, 30)) },
                { DayOfWeek.Wednesday, (new TimeOnly(13, 0), new TimeOnly(14, 30)) }
            };

            DateOnly startDate = new DateOnly(2024, 1, 1);
            DateOnly endDate = new DateOnly(2024, 4, 30);

            Schedule schedule1 = new Schedule(timeSlots, startDate, endDate);

            // Second Schedule
            var timeSlots2 = new Dictionary<DayOfWeek, (TimeOnly StartTime, TimeOnly EndTime)>
            {
                { DayOfWeek.Tuesday, (new TimeOnly(10, 0), new TimeOnly(11, 30)) },
                { DayOfWeek.Thursday, (new TimeOnly(14, 0), new TimeOnly(15, 30)) }
            };

            DateOnly startDate2 = new DateOnly(2024, 2, 1);
            DateOnly endDate2 = new DateOnly(2024, 5, 31);

            Schedule schedule2 = new Schedule(timeSlots2, startDate2, endDate2);
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Schedule> { schedule1, schedule2 });

            // Act
            var schedules = await scheduleService.GetAllSchedules();

            // Assert
            Assert.AreEqual(2, schedules.Count(), "Returned schedules should contain two schedules.");
            Assert.IsTrue(schedules.Contains(schedule1), "Schedule 1 not found in returned schedules.");
            Assert.IsTrue(schedules.Contains(schedule2), "Schedule 2 not found in returned schedules.");
        }
    }
}
