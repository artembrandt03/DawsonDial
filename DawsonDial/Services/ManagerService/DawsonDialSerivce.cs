using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.Enums;
using DawsonDial.Views;
using DawsonDial.Helpers;

namespace DawsonDial.Services.ManagerService
{
    /// <summary>
    /// This is a manager class that manages the other managers.
    /// </summary>
    public class DawsonDialService
    {
        private readonly UserService _userService;
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly AdminService _adminService;
        private readonly RoomService _roomService;
        private readonly EventService _eventService;
        private readonly CourseService _courseService;
        private readonly ScheduleService _scheduleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DawsonDialService"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="adminService">The admin service.</param>
        /// <param name="teacherService">The teacher service.</param>
        /// <param name="studentService">The student service.</param>
        /// <param name="roomService">The room service.</param>
        /// <param name="eventService">The event service.</param>
        /// <param name="courseService">The course service.</param>
        /// <param name="scheduleService">The schedule service.</param>
        public DawsonDialService(
            UserService userService,
            AdminService adminService,
            TeacherService teacherService,
            StudentService studentService,
            RoomService roomService,
            EventService eventService,
            CourseService courseService,
            ScheduleService scheduleService)
        {
            _userService = userService;
            _adminService = adminService;
            _teacherService = teacherService;
            _studentService = studentService;
            _roomService = roomService;
            _eventService = eventService;
            _courseService = courseService;
            _scheduleService = scheduleService;
        }

        /// <summary>
        /// Registers a user.
        /// </summary>
        /// <param name="person">The person.</param>
        public async Task RegisterUser(Person person)
        {
            await _userService.AddUser(person);
        }

        /// <summary>
        /// Gets a user by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The user with the given username.</returns>
        public async Task<Person> GetUserByUsername(string username)
        {
            return await _userService.GetUserByUsername(username);
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>All users.</returns>
        public async Task<IEnumerable<Person>> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsers();
            return new HashSet<Person>(users);
        }

        /// <summary>
        /// Updates a user's profile.
        /// </summary>
        /// <param name="person">The person.</param>
        public async Task UpdateUserProfile(Person person)
        {
            await _userService.UpdateUser(person);
        }

        /// <summary>
        /// Authenticates a user.
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>True if the user was successfully authenticated, false otherwise.</returns>
        public async Task<bool> AuthenticateUser(string username, string password)
        {
            return await _userService.Authenticate(username, password);
        }

        /// <summary>
        /// Changes a user's password.
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <param name="oldPassword">The user's old password.</param>
        /// <param name="newPassword">The user's new password.</param>
        public async Task ChangeUserPassword(string username, string oldPassword, string newPassword)
        {
            await _userService.UpdatePassword(username, oldPassword, newPassword);
        }

        /// <summary>
        /// Enrolls a student in a course.
        /// </summary>
        /// <param name="student">The student to enroll.</param>
        /// <param name="section">The section to enroll the student in.</param>
        public async Task EnrollStudentInCourse(Student student, Section section)
        {
            await _studentService.EnrollInCourse(student, section);
        }

        /// <summary>
        /// Drops a student from a course.
        /// </summary>
        /// <param name="student">The student to drop.</param>
        /// <param name="section">The section to drop the student from.</param>
        public async Task DropStudentFromCourse(Student student, Section section)
        {
            await _studentService.DropCourse(student, section);
        }

        /// <summary>
        /// Gets the schedule for a student.
        /// </summary>
        /// <param name="student">The student to get the schedule for.</param>
        /// <returns>The schedule for the student.</returns>
        public Schedule GetScheduleByStudent(Student student)
        {
            return student.Classes.First().Schedule;
        }

        /// <summary>
        /// Assigns a teacher to a course.
        /// </summary>
        /// <param name="teacher">The teacher to assign.</param>
        /// <param name="section">The section to assign the teacher to.</param>
        public async Task AssignTeacherToCourse(Teacher teacher, Section section)
        {
            await _teacherService.AddCourse(teacher, section);
        }

        /// <summary>
        /// Gets the available courses.
        /// </summary>
        /// <returns>The available courses.</returns>
        public async Task<HashSet<Course>> GetAvailableCourses()
        {
            var allCourses = await _courseService.GetAllCourses();
            return new HashSet<Course>(allCourses);
        }

        /// <summary>
        /// Creates a new course.
        /// </summary>
        /// <param name="course">The course to create.</param>
        public async Task AddCourse(Course course)
        {
            await _courseService.AddCourse(course);
        }

        /// <summary>
        /// Updates an existing course.
        /// </summary>
        /// <param name="course">The updated course object.</param>
        public async Task UpdateCourse(Course course)
        {
            await _courseService.UpdateCourse(course);
        }

        /// <summary>
        /// Gets all courses.
        /// </summary>
        /// <returns>All courses.</returns>
        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            var allCourses = await _courseService.GetAllCourses();
            return new HashSet<Course>(allCourses);
        }

        /// <summary>
        /// Sets the office hours for a teacher.
        /// </summary>
        /// <param name="teacher">The teacher to set office hours for.</param>
        /// <param name="officeHours">The office hours to set.</param>
        public async Task SetTeacherOfficeHours(Teacher teacher, OfficeHours officeHours)
        {
            await _teacherService.SetOfficeHoursAsync(teacher, officeHours);
        }

        /// <summary>
        /// Gets the schedule for a teacher.
        /// </summary>
        /// <param name="teacher">The teacher to get the schedule for.</param>
        /// <returns>The schedule for the teacher.</returns>
        public Schedule GetScheduleByTeacher(Teacher teacher)
        {
            return teacher.Classes.First().Schedule;
        }

        /// <summary>
        /// Gets the available events.
        /// </summary>
        /// <returns>The available events.</returns>
        public async Task<HashSet<Event>> GetAvailableEvents()
        {
            IEnumerable<Event> allEvents = await _eventService.GetAllEvents();
            return new HashSet<Event>(allEvents.Where(e => !e.IsFull));
        }
        
        /// <summary>
        /// Gets the available conferences.
        /// </summary>
        /// <returns>The available conferences.</returns>
        public async Task<HashSet<Conference>> GetAvailableConferences()
        {
            IEnumerable<Event> allEvents = await _eventService.GetAllEvents();
            return new HashSet<Conference>(allEvents.OfType<Conference>().Where(e => !e.IsFull));
        }

        /// <summary>
        /// Schedules an event.
        /// </summary>
        /// <param name="eventToSchedule">The event to schedule.</param>
        public async Task ScheduleEvent(Event eventToSchedule)
        {
            // Verify the room is actually available at the requested time
            bool isAvailable = await _roomService.IsRoomAvailableAt(
                eventToSchedule.Room,
                eventToSchedule.StartDateTime,
                eventToSchedule.EndDateTime - eventToSchedule.StartDateTime);

            if (!isAvailable)
            {
                throw new InvalidOperationException("This room is not available at the requested time.");
            }

            // Reserve the room in the database
            await _roomService.ReserveRoom(
                eventToSchedule.Room,
                eventToSchedule,
                eventToSchedule.StartDateTime,
                eventToSchedule.EndDateTime - eventToSchedule.StartDateTime);

            // Save event to database
            await _eventService.AddNewEvent(eventToSchedule);
        }

        /// <summary>
        /// Cancels an event.
        /// </summary>
        /// <param name="eventToCancel">The event to cancel.</param>
        public async Task CancelEvent(Event eventToCancel)
        {
            await _eventService.ChangeEventStatus(eventToCancel, EventStatus.Cancelled);
            await _roomService.ReleaseRoom(eventToCancel.Room, eventToCancel.StartDateTime);
        }

        /// <summary>
        /// Updates an event.
        /// <param name="eventToUpdate">The event to update.</param>
        public async Task UpdateEventAsync(Event eventToUpdate)
        {
            await _eventService.UpdateEvent(eventToUpdate);
        }

        /// <summary>
        /// Gets all events.
        /// </summary>
        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            var events = await _eventService.GetAllEvents();
            return events;
        }

        /// <summary>
        /// Registers a user for an event.
        /// </summary>
        /// <param name="person">The person to register.</param>
        /// <param name="eventToAttend">The event to attend.</param>
        public async Task RegisterUserForEvent(Person person, Event eventToAttend)
        {
            await _eventService.AddParticipantToEvent(eventToAttend, person);
        }

        /// <summary>
        /// Adds a schedule to the system.
        /// </summary>
        /// <param name="schedule">The schedule to add.</param>
        /// <returns>Async task</returns>
        public async Task AddScheduleAsync(Schedule schedule)
        {
            await _scheduleService.AddSchedule(schedule);
        }

        /// <summary>
        /// Unregisters a user from an event.
        /// </summary>
        /// <param name="person">The person to unregister.</param>
        /// <param name="eventToUnattend">The event to unregister from.</param>
        public async Task UnregisterUserFromEvent(Person person, Event eventToUnattend)
        {
            await _eventService.RemoveParticipantFromEvent(eventToUnattend, person);
        }

        /// <summary>
        /// Adds a room to the system.
        /// </summary>
        /// <param name="room">The room to add.</param>
        public async Task AddRoomAsync(Room room)
        {
            await _roomService.AddRoomAsync(room);
        }

        /// <summary>
        /// Removes a room from the system.
        /// </summary>
        /// <param name="room">The room to remove.</param>
        public async Task UpdateRoomAsync(Room room)
        {
            await _roomService.UpdateRoomAsync(room);
        }

        /// <summary>
        /// Reserves a room for an event.
        /// </summary>
        /// <param name="room">The room to reserve.</param>
        /// <param name="eventToSchedule">The event to schedule.</param>
        /// <param name="dateTime">The date and time of the event.</param>
        /// <param name="duration">The duration of the event.</param>
        /// <returns>True if the room was successfully reserved, false otherwise.</returns>
        public async Task<bool> ReserveRoom(Room room, Event eventToSchedule, DateTime dateTime, TimeSpan duration)
        {
            return await _roomService.ReserveRoom(room, eventToSchedule, dateTime, duration);
        }

        /// <summary>
        /// Checks if a room is available at a given date and time.
        /// </summary>
        /// <param name="room">The room to check</param>
        /// <param name="dateTime">The date and time of the start</param>
        /// <param name="duration">The duration</param>
        /// <returns></returns>
        public async Task<bool> IsRoomAvailableAt(Room room, DateTime dateTime, TimeSpan duration)
        {
            return await _roomService.IsRoomAvailableAt(room, dateTime, duration);
        }

        /// <summary>
        /// Returns a set of available rooms at a given date and time.
        /// </summary>
        /// <param name="dateTime">The date and time to check availability.</param>
        /// <returns>A set of available rooms.</returns>
        public async Task<HashSet<Room>> GetAvailableRoomsAt(DateTime dateTime)
        {
            return await _roomService.AvailableRoomsAtAsync(dateTime);
        }

        /// <summary>
        /// Returns a list of all rooms.
        /// </summary>
        /// <returns>A list of all rooms.</returns>
        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _roomService.GetAllRoomsAsync();
        }

        /// <summary>
        /// Returns a list of all admin logs.
        /// </summary>
        /// <returns>A list of all admin logs.</returns>
        public async Task<IEnumerable<AdminLog>> GetAllLogsAsync()
        {
            return await _adminService.GetAllLogsAsync();
        }

        /// <summary>
        /// Logs an action performed by an admin.
        /// </summary>
        /// <param name="admin">The admin performing the action.</param>
        /// <param name="action">The action performed.</param>
        public async Task LogAction(Admin admin, string action)
        {
            await _adminService.LogAction(admin, action);
        }

        /// <summary>
        /// Picks a room from the available rooms.
        /// </summary>
        /// <returns>The selected room.</returns>
        public async Task<Room> PickRoom()
        {
            var rooms = (await _roomService.GetAllRoomsAsync()).ToList();
            if (!rooms.Any()) throw new InvalidOperationException("No rooms available.");

            AdminViews.PrintRooms(rooms);
            int choice = InputPrompter.PromptInt("\nSelect a room (or 0 to go back): ", 0, rooms.Count);
            return choice == 0 ? null! : rooms[choice - 1];
        }

        /// <summary>
        /// Picks a course from the available courses.
        /// </summary>
        /// <returns>The selected course.</returns>
        public async Task<Course> PickCourse()
        {
            var courses = (await _courseService.GetAllCourses()).ToList();
            if (!courses.Any()) throw new InvalidOperationException("No courses available.");

            AdminViews.PrintCourses(courses);
            int choice = InputPrompter.PromptInt("\nSelect a course (or 0 to go back): ", 0, courses.Count);
            return choice == 0 ? null! : courses[choice - 1];
        }

        /// <summary>
        /// Picks a teacher from the available teachers.
        /// </summary>
        /// <returns>The selected teacher.</returns>
        public async Task<Teacher> PickTeacher()
        {
            var teachers = (await _teacherService.GetAllUsers()).Cast<Teacher>().ToList();
            if (!teachers.Any()) throw new InvalidOperationException("No teachers available.");

            AdminViews.PrintUsers(teachers, "Teacher");
            int choice = InputPrompter.PromptInt("\nSelect a teacher (or 0 to go back): ", 0, teachers.Count);
            return choice == 0 ? null! : teachers[choice - 1];
        }

        /// <summary>
        /// Picks a person from the available people.
        /// </summary>
        /// <returns>The selected person.</returns>
        public async Task<Person> PickPerson()
        {
            var people = (await _userService.GetAllUsers()).ToList();
            if (!people.Any()) throw new InvalidOperationException("No users available.");

            AdminViews.PrintUsers(people, "User");
            int choice = InputPrompter.PromptInt("\nSelect a person: ", 1, people.Count);
            return people[choice - 1];
        }

        //----------------------------------------------------------------------------------------------------------
        //For GUI
        // In DawsonDialService.cs
        public async Task CreateStudent(string username, string password, string firstName, string lastName, int age, int studentId, string program, int year)
        {
            await _userService.CreateStudent(username, password, firstName, lastName, age, studentId, program, year);
        }

        public async Task CreateTeacher(string username, string password, string firstName, string lastName, int age, string department)
        {
            await _userService.CreateTeacher(username, password, firstName, lastName, age, department);
        }

        public async Task CreateAdmin(string username, string password, string firstName, string lastName, int age)
        {
            await _userService.CreateAdmin(username, password, firstName, lastName, age);
        }

        public async Task AddSchedule(Schedule schedule)
        {
            ArgumentNullException.ThrowIfNull(schedule);
            await _scheduleService.AddSchedule(schedule);
        }
        public async Task AddSection(Section section)
        {
            await _courseService.AddSection(section); 
        }

        public async Task<List<Teacher>> GetAllTeachers()
        {
            var allUsers = await _teacherService.GetAllUsers();
            return allUsers.OfType<Teacher>().ToList(); // Safe cast
        }
        
        /// <summary>
        /// Automatically marks all past events as Completed if they haven't been cancelled.
        /// </summary>
        public async Task MarkPastEventsAsCompleted()
        {
            await _roomService.MarkPastEventsAsCompleted();
        }

        /// <summary>
        /// Gets a teacher with all related schedules and time slots.
        /// </summary>
        public async Task<Teacher> GetTeacherWithSchedulesAsync(Guid teacherId)
        {
            return await _teacherService.GetTeacherWithSchedulesAsync(teacherId);
        }

        public async Task<Student> GetStudentWithSchedulesAsync(Guid studentId)
        {
            return await _studentService.GetStudentWithSchedulesAsync(studentId);
        }
    }
}
