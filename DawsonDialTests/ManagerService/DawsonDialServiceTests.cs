using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.Enums;
using DawsonDial.Services.ManagerService;
using DawsonDial.Repositories.Interfaces;

namespace DawsonDialTests
{
    /// <summary>
    /// Tests for DawsonDialService.
    /// </summary>
    /// FIX:
    /// There was two key problems here:
    /// 1)We initially attempted to mock concrete classes using Moq, but it's impossible to do
    /// with classes that don't have a parameterless constructor, which caused a MissingMethodException. 
    /// Resolved this by using real service instances for each dependency while mocking their required repositories! 
    /// 2)Failing test (CancelEvent_ShouldSetStatusToCancelledAndCallUpdateEvent) caused by setting the event's StartDateTime to DateTime.Now. 
    /// This made the ReleaseRoom method throw an exception, as it does not allow releasing rooms in the past. 
    /// Fixed this by updating the test to use DateTime.Now.AddMinutes(1) to ensure the timestamp was safely in the future.
    [TestClass]
    public class DawsonDialServiceTests
    {
        private DawsonDialService service = null!;

        // Repositories
        private Mock<IPersonRepository> personRepoMock = null!;
        private Mock<IAdminLogRepository> adminLogRepoMock = null!;
        private Mock<IRoomRepository> roomRepoMock = null!;
        private Mock<IEventRepository> eventRepoMock = null!;
        private Mock<ICourseRepository> courseRepoMock = null!;
        private Mock<ISectionRepository> sectionRepoMock = null!;
        private Mock<IScheduleRepository> scheduleRepoMock = null!;

        private static readonly DateTime TestDateTime = DateTime.Now;
        private static readonly TimeSpan TestDuration = TimeSpan.FromMinutes(30);

        [TestInitialize]
        public void Setup()
        {
            personRepoMock = new Mock<IPersonRepository>();
            adminLogRepoMock = new Mock<IAdminLogRepository>();
            roomRepoMock = new Mock<IRoomRepository>();
            eventRepoMock = new Mock<IEventRepository>();
            courseRepoMock = new Mock<ICourseRepository>();
            sectionRepoMock = new Mock<ISectionRepository>();
            scheduleRepoMock = new Mock<IScheduleRepository>();

            var userService = new UserService(personRepoMock.Object);
            var teacherService = new TeacherService(personRepoMock.Object);
            var studentService = new StudentService(personRepoMock.Object);
            var adminService = new AdminService(personRepoMock.Object, adminLogRepoMock.Object);
            var roomService = new RoomService(roomRepoMock.Object, eventRepoMock.Object);
            var eventService = new EventService(eventRepoMock.Object);
            var courseService = new CourseService(courseRepoMock.Object, sectionRepoMock.Object);
            var scheduleService = new ScheduleService(scheduleRepoMock.Object);

            service = new DawsonDialService(
                userService,
                adminService,
                teacherService,
                studentService,
                roomService,
                eventService,
                courseService,
                scheduleService);
        }

        [TestMethod]
        public void GetScheduleByTeacher_ShouldReturnSchedule()
        {
            //Arrange
            var expectedSchedule = new Mock<Schedule>().Object;
            var sectionMock = new Mock<Section>();
            sectionMock.Setup(s => s.Schedule).Returns(expectedSchedule);
            var sections = new HashSet<Section> { sectionMock.Object };

            var teacherMock = new Mock<Teacher>();
            teacherMock.Setup(t => t.Classes).Returns(sections);

            //Act
            var result = service.GetScheduleByTeacher(teacherMock.Object);

            //Assert
            Assert.AreEqual(expectedSchedule, result);
        }

        [TestMethod]
        public async Task CancelEvent_ShouldSetStatusToCancelledAndCallUpdateEvent()
        {
            //Arrange
            var futureTime = DateTime.Now.AddMinutes(1); // ensure it's in the future
            var eventMock = new Mock<Event>();
            var office = new Office("3A.20", 10, true);

            eventMock.SetupProperty(e => e.Status, EventStatus.Planned);
            eventMock.Setup(e => e.Room).Returns(office);
            eventMock.Setup(e => e.StartDateTime).Returns(futureTime);

            var eventToCancel = eventMock.Object;

            //Act
            await service.CancelEvent(eventToCancel);

            //Assert
            Assert.AreEqual(EventStatus.Cancelled, eventToCancel.Status);
        }
    }
}