using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.Enums;
using DawsonDial.Helpers;
using DawsonDial.Views;
using DawsonDial.Services.ManagerService;

namespace DawsonDial.Controllers
{
    /// <summary>
    /// Represents the admin controller.
    /// </summary>
    public class AdminController
    {
        private readonly DawsonDialService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/> class.
        /// </summary>
        /// <param name="service">The service instance.</param>
        public AdminController(DawsonDialService service)
        {
            _service = service;
        }

        /// <summary>
        /// Updates the profile of an admin.
        /// </summary>
        /// <param name="admin">The admin.</param>
        public async Task UpdateProfileAsync(Admin admin)
        {
            Console.WriteLine("\n===== Update Profile =====");
            Console.WriteLine("Leave blank to keep current value.");

            string firstName = InputPrompter.Prompt($"First Name [{admin.FirstName}]: ");
            if (!string.IsNullOrWhiteSpace(firstName)) admin.FirstName = firstName;

            string lastName = InputPrompter.Prompt($"Last Name [{admin.LastName}]: ");
            if (!string.IsNullOrWhiteSpace(lastName)) admin.LastName = lastName;

            string ageStr = InputPrompter.Prompt($"Age [{admin.Age}]: ");
            if (int.TryParse(ageStr, out int age)) admin.Age = age;

            string desc = InputPrompter.Prompt($"Description [{admin.Description}]: ");
            if (!string.IsNullOrWhiteSpace(desc)) admin.Description = desc;

            try
            {
                await _service.UpdateUserProfile(admin);
                Console.WriteLine("\nProfile updated successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError updating profile: {e.Message}");
            }
        }

        /// <summary>
        /// Changes the password for an admin.
        /// </summary>
        /// <param name="admin">The admin whose password is to be changed.</param>
        public async Task ChangePasswordAsync(Admin admin)
        {
            Console.WriteLine("\n===== Change Password =====");
            string current = InputPrompter.Prompt("Current Password: ");
            string newPass = InputPrompter.Prompt("New Password: ");
            string confirm = InputPrompter.Prompt("Confirm New Password: ");

            if (newPass != confirm)
            {
                Console.WriteLine("\nPasswords do not match.");
                return;
            }

            try
            {
                await _service.ChangeUserPassword(admin.Username, current, newPass);
                Console.WriteLine("\nPassword updated successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError changing password: {e.Message}");
            }
        }

        /// <summary>
        /// Allows admin to create a new user.
        /// </summary>
        /// <param name="admin">The admin creating the user.</param>
        public async Task CreateUser(Admin admin)
        {
            AdminViews.CreateUserMenu();
            int choice = InputPrompter.PromptInt("Select user type: ", 1, 4);
            if (choice == 4)
            {
                Console.WriteLine("\nCancelled user creation.");
                return;
            }
            switch (choice)
            {
                case 1:
                    await CreateStudent(admin);
                    break;
                case 2:
                    await CreateTeacher(admin);
                    break;
                case 3:
                    await CreateAdmin(admin);
                    break;
                case 4:
                    break;
            }
        }

        /// <summary>
        /// Helper method to create a new student.
        /// </summary>
        /// <param name="admin">The admin user creating the student.</param>
        private async Task CreateStudent(Admin admin)
        {
            try
            {
                string username = InputPrompter.Prompt("Enter username: ");
                string password = InputPrompter.Prompt("Enter password: ");
                string firstName = InputPrompter.Prompt("Enter first name: ");
                string lastName = InputPrompter.Prompt("Enter last name: ");
                int age = InputPrompter.PromptInt("Enter age: ", 10, 100);
                int studentID = InputPrompter.PromptInt("Enter student ID: ", 1000000, 9999999);
                string program = InputPrompter.Prompt("Enter program: ");
                int year = InputPrompter.PromptInt("Enter year: ", 1, 3);

                var student = new Student(
                    username,
                    password,
                    firstName,
                    lastName,
                    age,
                    isDisabled: false,
                    description: "",
                    studentID,
                    program,
                    year,
                    new HashSet<Section>());

                AdminViews.ConfirmUserCreation(student);

                if (InputPrompter.Confirm("Confirm creation?"))
                {
                    await _service.RegisterUser(student);
                    Console.WriteLine("\nStudent created successfully.");
                    admin.IncrementUserCreationCount();
                    await _service.LogAction(admin, $"Created student {student.Username}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError creating student: {ex.Message}");
            }
        }

        /// <summary>
        /// Helper method to create a new teacher.
        /// </summary>
        private async Task CreateTeacher(Admin admin)
        {
            try
            {
                string username = InputPrompter.Prompt("Enter username: ");
                string password = InputPrompter.Prompt("Enter password: ");
                string firstName = InputPrompter.Prompt("Enter first name: ");
                string lastName = InputPrompter.Prompt("Enter last name: ");
                int age = InputPrompter.PromptInt("Enter age: ", 20, 100);
                string department = InputPrompter.Prompt("Enter department: ");

                var teacher = new Teacher(
                    username,
                    password,
                    firstName,
                    lastName,
                    age,
                    isDisabled: false,
                    description: "",
                    department,
                    new HashSet<Section>(),
                    officeRoom: null!,
                    officeHours: null!
                );

                AdminViews.ConfirmUserCreation(teacher);

                if (InputPrompter.Confirm("Confirm creation?"))
                {
                    await _service.RegisterUser(teacher);
                    Console.WriteLine("\nTeacher created successfully.");
                    admin.IncrementUserCreationCount();
                    await _service.LogAction(admin, $"Created teacher {teacher.Username}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError creating teacher: {ex.Message}");
            }
        }

        /// <summary>
        /// Helper method to create a new admin.
        /// </summary>
        private async Task CreateAdmin(Admin admin)
        {
            try
            {
                string username = InputPrompter.Prompt("Enter username: ");
                string password = InputPrompter.Prompt("Enter password: ");
                string firstName = InputPrompter.Prompt("Enter first name: ");
                string lastName = InputPrompter.Prompt("Enter last name: ");
                int age = InputPrompter.PromptInt("Enter age: ", 20, 100);

                var newAdmin = new Admin(
                    username,
                    password,
                    firstName,
                    lastName,
                    age,
                    isDisabled: false,
                    description: ""
                );

                AdminViews.ConfirmUserCreation(newAdmin);

                if (InputPrompter.Confirm("Confirm creation?"))
                {
                    await _service.RegisterUser(newAdmin);
                    Console.WriteLine("\nAdmin created successfully.");
                    newAdmin.IncrementUserCreationCount();
                    await _service.LogAction(admin, $"Created admin {newAdmin.Username}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError creating admin: {ex.Message}");
            }
        }

        /// <summary>
        /// Allows admin to create a new course.
        /// </summary>
        /// <param name="admin">The admin creating the course.</param>
        public async Task CreateCourse(Admin admin)
        {
            try
            {
                Console.WriteLine("\n===== Create Course =====");

                string courseCode = InputPrompter.Prompt("Enter course code (e.g., COMP-1234): ");
                string subject = InputPrompter.Prompt("Enter subject name (e.g., Programming 1): ");

                var course = new Course(
                    courseCode,
                    subject,
                    new HashSet<Teacher>(),
                    new HashSet<Section>()
                );

                AdminViews.ConfirmCourseCreation(course);

                if (InputPrompter.Confirm("Confirm creation?"))
                {
                    await _service.AddCourse(course);
                    Console.WriteLine("\nCourse created successfully.");
                    await _service.LogAction(admin, $"Created course {course.CourseCode}");
                }
                else
                {
                    Console.WriteLine("\nCourse creation cancelled.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError creating course: {ex.Message}");
            }
        }

        /// <summary>
        /// Allows admin to create a new room.
        /// </summary>
        /// <param name="admin">The admin creating the room.</param>
        public async Task CreateRoom(Admin admin)
        {
            try
            {
                AdminViews.CreateRoomMenu();
                int choice = InputPrompter.PromptInt("Select room type: ", 1, 4);
                if (choice == 4)
                {
                    Console.WriteLine("\nCancelled room creation.");
                    return;
                }

                string roomNumber = InputPrompter.Prompt("Enter room number (e.g., 2E-14): ");
                int seatCount = InputPrompter.PromptInt("Enter number of seats: ", 1, 300);

                Room newRoom;
                switch (choice)
                {
                    case 1: // Conference Room
                        bool hasProjectorConf = InputPrompter.Confirm("Does the room have a projector?");
                        newRoom = new ConferenceRoom(roomNumber, seatCount, hasProjectorConf);
                        break;
                    case 2: // Classroom
                        bool hasProjectorClass = InputPrompter.Confirm("Does the classroom have a projector?");
                        bool hasComputers = InputPrompter.Confirm("Does the classroom have computers?");
                        newRoom = new ClassRoom(roomNumber, seatCount, hasProjectorClass, hasComputers);
                        break;
                    case 3: // Office
                        bool isShared = InputPrompter.Confirm("Is this office shared?");
                        newRoom = new Office(roomNumber, seatCount, isShared);
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        return;
                }

                AdminViews.ConfirmRoomCreation(newRoom);

                if (InputPrompter.Confirm("Confirm room creation?"))
                {
                    await _service.AddRoomAsync(newRoom);
                    Console.WriteLine("\nRoom created successfully.");
                    await _service.LogAction(admin, $"Created room {newRoom.RoomNumber}");
                }
                else
                {
                    Console.WriteLine("\nRoom creation cancelled.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError creating room: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new event.
        /// </summary>
        /// <param name="admin">The admin user creating the event.</param>
        public async Task CreateEvent(Admin admin)
        {
            try
            {
                AdminViews.CreateEventMenu();
                int choice = InputPrompter.PromptInt("Select event type: ", 1, 3);
                if (choice == 3)
                {
                    Console.WriteLine("\nCancelled event creation.");
                    return;
                }

                string title = InputPrompter.Prompt("Event Title: ");
                string description = InputPrompter.Prompt("Event Description: ");

                DateOnly date = InputPrompter.PromptDate("Date (MM/DD/YYYY): ");
                TimeOnly startTime = InputPrompter.PromptTime("Start Time (HH:MM): ");
                int durationMinutes = InputPrompter.PromptInt("Duration in minutes: ", 1);
                bool isRecurring = InputPrompter.Confirm("Is event recurring?");

                DateTime startDateTime = date.ToDateTime(startTime).ToUniversalTime();
                DateTime endDateTime = startDateTime.AddMinutes(durationMinutes).ToUniversalTime();
                TimeSpan duration = endDateTime - startDateTime;

                Room room = await _service.PickRoom();
                Event newEvent;

                switch (choice)
                {
                    case 1: // Conference
                        Person speaker = await _service.PickPerson();
                        newEvent = new Conference(
                            title, description, room,
                            startDateTime, endDateTime, isRecurring, EventStatus.Planned, speaker
                        );
                        break;

                    case 2: // OfficeMeeting
                        Teacher teacher = await _service.PickTeacher();
                        newEvent = new OfficeMeeting(
                            title, description, room,
                            startDateTime, endDateTime, isRecurring, EventStatus.Planned, teacher
                        );
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        return;
                }

                AdminViews.ConfirmEventCreation(newEvent);

                if (InputPrompter.Confirm("\nConfirm event creation?"))
                {
                    await _service.ScheduleEvent(newEvent);
                    Console.WriteLine("\nEvent created successfully.");
                    await _service.LogAction(admin, $"Created event {newEvent.Title}");
                }
                else
                {
                    Console.WriteLine("\nEvent creation cancelled.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred while creating the event: {ex.Message}");
            }
        }

        /// <summary>
        /// Allows admin to edit a user.
        /// </summary>
        /// <param name="admin">The admin editing the user.</param>
        public async Task EditUser(Admin admin)
        {
            Console.WriteLine("\n===== Edit User =====");

            //Get all users except the current admin themselves
            var users = (await _service.GetAllUsersAsync()).Where(u => u.PersonId != admin.PersonId).ToList();

            //If no users are found, we will notify and return this
            if (!users.Any())
            {
                Console.WriteLine("\nNo users available to edit!");
                return;
            }

            //Display all users
            AdminViews.PrintUsers(users, "User");

            //Ask admin to select a user to edit by index
            int index = InputPrompter.PromptInt("\nSelect a user by number to edit (or 0 to go back): ", 0, users.Count);
            if (index == 0)
                return;
            var selectedUser = users[index - 1];

            //Show current user info
            Console.WriteLine($"\nEditing User: {selectedUser.FirstName} {selectedUser.LastName} [{selectedUser.Username}]");
            Console.WriteLine("Leave fields blank to keep current values.\n");

            //Prompt for new values for the user (skip any blank input)
            string newFirstName = InputPrompter.Prompt($"First Name [{selectedUser.FirstName}]: ");
            string newLastName = InputPrompter.Prompt($"Last Name [{selectedUser.LastName}]: ");
            string newAgeStr = InputPrompter.Prompt($"Age [{selectedUser.Age}]: ");
            string newDescription = InputPrompter.Prompt($"Description [{selectedUser.Description ?? "None"}]: ");

            //Apply new values if they were provided
            if (!string.IsNullOrWhiteSpace(newFirstName)) selectedUser.FirstName = newFirstName;
            if (!string.IsNullOrWhiteSpace(newLastName)) selectedUser.LastName = newLastName;
            if (int.TryParse(newAgeStr, out int newAge)) selectedUser.Age = newAge;
            if (!string.IsNullOrWhiteSpace(newDescription)) selectedUser.Description = newDescription;

            //Confirm and update user
            if (InputPrompter.Confirm("Confirm update?"))
            {
                try
                {
                    await _service.UpdateUserProfile(selectedUser);
                    Console.WriteLine("\nUser updated successfully.");
                    await _service.LogAction(admin, $"Updated user {selectedUser.Username}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError updating user: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("\nUpdate cancelled.");
            }
        }

        /// <summary>
        /// Allows admin to edit a course.
        /// </summary>
        /// <param name="admin">The admin editing the course.</param>
        public async Task EditCourse(Admin admin)
        {
            Console.WriteLine("\n===== Edit Course =====");

            //Get all courses
            var courses = (await _service.GetAllCoursesAsync()).ToList();

            //Handle case where no courses exist
            if (!courses.Any())
            {
                Console.WriteLine("\nNo courses available to edit!");
                return;
            }

            //Display course list to admin
            AdminViews.PrintCourses(courses);

            //Ask admin to choose a course by index
            int index = InputPrompter.PromptInt("Select a course by number to edit (or 0 to go back): ", 0, courses.Count);
            if (index == 0)
                return;
            var selectedCourse = courses[index - 1];

            //Show current course details
            Console.WriteLine($"\nEditing Course: {selectedCourse.CourseCode} - {selectedCourse.Subject}");
            Console.WriteLine("Leave fields blank to keep current values.\n");

            //Prompt for updated course code and subject
            string newCode = InputPrompter.Prompt($"Course Code [{selectedCourse.CourseCode}]: ");
            string newSubject = InputPrompter.Prompt($"Subject [{selectedCourse.Subject}]: ");

            //Apply new values if provided
            if (!string.IsNullOrWhiteSpace(newCode)) selectedCourse.CourseCode = newCode;
            if (!string.IsNullOrWhiteSpace(newSubject)) selectedCourse.Subject = newSubject;

            //Confirm update
            if (InputPrompter.Confirm("Confirm update?"))
            {
                try
                {
                    await _service.UpdateCourse(selectedCourse);
                    Console.WriteLine("\nCourse updated successfully.");
                    await _service.LogAction(admin, $"Updated course {selectedCourse.CourseCode} - {selectedCourse.Subject}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError updating course: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("\nUpdate cancelled.");
            }
        }

        /// <summary>
        /// Allows admin to edit a room.
        /// </summary>
        /// <param name="admin">The admin editing the room.</param>
        public async Task EditRoom(Admin admin)
        {
            try
            {
                //Prompting admin to select a room from the system (using DDS)
                var room = await _service.PickRoom();
                if (room == null) return;

                Console.WriteLine($"\nEditing Room: {room.RoomNumber} ({room.GetType().Name})");
                Console.WriteLine("Leave fields blank to retain existing values.");

                //Prompting to edit RoomNumber
                string newRoomNumber = InputPrompter.Prompt($"Room Number [{room.RoomNumber}]: ");
                if (!string.IsNullOrWhiteSpace(newRoomNumber)) room.RoomNumber = newRoomNumber;

                //Prompting to edit the number of seats
                string newNumOfSeats = InputPrompter.Prompt($"Number of seats [{room.NumberOfSeats}]: ");
                if (int.TryParse(newNumOfSeats, out int newSeatCount))
                {
                    room.NumberOfSeats = newSeatCount;
                }

                //Handle some subclass-specific attributes
                switch (room)
                {
                    case ClassRoom classRoom:
                        classRoom.HasProjector = InputPrompter.Confirm($"Has Projector? (Current: {classRoom.HasProjector})");
                        classRoom.HasComputers = InputPrompter.Confirm($"Has Computers? (Current: {classRoom.HasComputers})");
                        break;

                    case ConferenceRoom confRoom:
                        confRoom.HasProjector = InputPrompter.Confirm($"Has Projector? (Current: {confRoom.HasProjector})");
                        break;

                    case Office office:
                        office.Shared = InputPrompter.Confirm($"Is Shared? (Current: {office.Shared})");
                        break;
                }

                //Confirming and applying update
                AdminViews.ConfirmRoomCreation(room);
                if (InputPrompter.Confirm("Apply changes to this room?"))
                {
                    await _service.UpdateRoomAsync(room);
                    Console.WriteLine("\nRoom updated successfully!");
                    await _service.LogAction(admin, $"Updated room {room.RoomNumber}");
                }
                else
                {
                    Console.WriteLine("\nRoom update cancelled!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError occurred while editing the room: {ex.Message}");
            }
        }

        /// <summary>
        /// Allows admin to edit an event.
        /// </summary>
        /// <param name="admin">The admin editing the event.</param>
        public async Task EditEvent(Admin admin)
        {
            try
            {
                //admin selecting the event to edit
                var events = (await _service.GetAllEventsAsync()).ToList();
                if (!events.Any())
                {
                    Console.WriteLine("\nNo events available to edit.");
                    return;
                }

                AdminViews.PrintEvents(events);
                int choice = InputPrompter.PromptInt("\nSelect an event to edit (or 0 to go back): ", 0, events.Count);
                if (choice == 0)
                    return;

                var selectedEvent = events[choice - 1];

                Console.WriteLine($"\nEditing Event: {selectedEvent.Title}");

                //Prompting to edit title
                string newTitle = InputPrompter.Prompt($"Title [{selectedEvent.Title}]: ");
                if (!string.IsNullOrWhiteSpace(newTitle))
                    selectedEvent.Title = newTitle;

                //Prompting to edit description
                string newDesc = InputPrompter.Prompt($"Description [{selectedEvent.Description}]: ");
                if (!string.IsNullOrWhiteSpace(newDesc))
                    selectedEvent.Description = newDesc;

                //Prompting to edit date and time
                string dateInput = InputPrompter.Prompt($"Start Date [{selectedEvent.StartDateTime:MM/dd/yyyy}]: ");
                string timeInput = InputPrompter.Prompt($"Start Time [{selectedEvent.StartDateTime:HH:mm}]: ");
                string durationInput = InputPrompter.Prompt($"Duration in minutes [{(selectedEvent.EndDateTime - selectedEvent.StartDateTime).TotalMinutes}]: ");

                DateOnly newDate = string.IsNullOrWhiteSpace(dateInput)
                    ? DateOnly.FromDateTime(selectedEvent.StartDateTime)
                    : DateOnly.Parse(dateInput);

                TimeOnly newStartTime = string.IsNullOrWhiteSpace(timeInput)
                    ? TimeOnly.FromDateTime(selectedEvent.StartDateTime)
                    : TimeOnly.Parse(timeInput);

                TimeSpan newDuration = string.IsNullOrWhiteSpace(durationInput)
                    ? selectedEvent.EndDateTime - selectedEvent.StartDateTime
                    : TimeSpan.FromMinutes(int.Parse(durationInput));

                DateTime newStartDateTime = DateTime.SpecifyKind(newDate.ToDateTime(newStartTime), DateTimeKind.Utc);
                DateTime newEndDateTime = newStartDateTime + newDuration;

                selectedEvent.StartDateTime = newStartDateTime;
                selectedEvent.EndDateTime = newEndDateTime;

                //let admin reassign the room
                if (InputPrompter.Confirm("Change the room?"))
                {
                    var newRoom = await _service.PickRoom();
                    selectedEvent.Room = newRoom;
                }

                //Confirming and saving the updated event
                AdminViews.ConfirmEventCreation(selectedEvent);
                if (InputPrompter.Confirm("Apply changes to this event?"))
                {
                    await _service.UpdateEventAsync(selectedEvent);
                    Console.WriteLine("\nEvent updated successfully.");
                    await _service.LogAction(admin, $"Updated event {selectedEvent.Title}");
                }
                else
                {
                    Console.WriteLine("\nEvent update cancelled.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred while editing the event: {ex.Message}");
            }
        }

        /// <summary>
        /// Allows admin to view all users.
        /// </summary>
        /// <param name="admin">The admin viewing the users.</param>
        public async Task ViewUsers(Admin admin)
        {
            var users = await _service.GetAllUsersAsync();
            AdminViews.PrintUsers(users, "User");
        }

        /// <summary>
        /// Allows admin to view all courses.
        /// </summary>
        /// <param name="admin">The admin viewing the courses.</param>
        public async Task ViewCourses(Admin admin)
        {
            var courses = await _service.GetAllCoursesAsync();
            AdminViews.PrintCourses(courses);
        }

        /// <summary>
        /// Allows admin to view all rooms.
        /// </summary>
        /// <param name="admin">The admin viewing the rooms.</param>
        public async Task ViewRooms(Admin admin)
        {
            var rooms = await _service.GetAllRoomsAsync();
            AdminViews.PrintRooms(rooms);
        }

        /// <summary>
        /// Allows admin to view all events.
        /// </summary>
        /// <param name="admin">The admin viewing the events.</param>
        public async Task ViewEvents(Admin admin)
        {
            var events = await _service.GetAllEventsAsync();
            AdminViews.PrintEvents(events);
        }

        /// <summary>
        /// Allows admin to view logs.
        /// </summary>
        /// <param name="admin">The admin viewing the logs.</param>
        public async Task ViewLogs(Admin admin)
        {
            var logs = await _service.GetAllLogsAsync();
            AdminViews.PrintLogs(logs);
        }
    }
}
