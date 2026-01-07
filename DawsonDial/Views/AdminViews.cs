using DawsonDial.Controllers;
using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;

namespace DawsonDial.Views
{
    /// <summary>
    /// Provides methods for displaying admin-related views.
    /// </summary>
    public static class AdminViews
    {
        /// <summary>
        /// Displays the admin menu and handles user choices.
        /// </summary>
        /// <param name="admin">The admin user.</param>
        /// <param name="controller">The admin controller.</param>
        /// <param name="authController">The authentication controller.</param>
        public static async Task ShowAsync(Admin admin, AdminController controller, AuthController authController)
        {
            bool exit = false;
            admin.UpdateLastLogin();

            while (!exit)
            {
                Console.WriteLine($"\n===== Admin Menu [{admin.FirstName} {admin.LastName}] =====");
                Console.WriteLine("1.  View Profile");
                Console.WriteLine("2.  Update Profile");
                Console.WriteLine("3.  Change Password");
                Console.WriteLine("4.  Create User");
                Console.WriteLine("5.  Create Course");
                Console.WriteLine("6.  Create Room");
                Console.WriteLine("7.  Create Event");
                Console.WriteLine("8.  Edit User");
                Console.WriteLine("9.  Edit Course");
                Console.WriteLine("10. Edit Room");
                Console.WriteLine("11. Edit Event");
                Console.WriteLine("12. View Users");
                Console.WriteLine("13. View Course");
                Console.WriteLine("14. View Rooms");
                Console.WriteLine("15. View Events");
                Console.WriteLine("16. View Logs");
                Console.WriteLine("17. Logout");
                Console.Write("> ");

                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        ShowProfile(admin);
                        break;
                    case "2":
                        await controller.UpdateProfileAsync(admin);
                        break;
                    case "3":
                        await controller.ChangePasswordAsync(admin);
                        break;
                    case "4":
                        await controller.CreateUser(admin);
                        break;
                    case "5":
                        await controller.CreateCourse(admin);
                        break;
                    case "6":
                        await controller.CreateRoom(admin);
                        break;
                    case "7":
                        await controller.CreateEvent(admin);
                        break;
                    case "8":
                        await controller.EditUser(admin);
                        break;
                    case "9":
                        await controller.EditCourse(admin);
                        break;
                    case "10":
                        await controller.EditRoom(admin);
                        break;
                    case "11":
                        await controller.EditEvent(admin);
                        break;
                    case "12":
                        await controller.ViewUsers(admin);
                        break;
                    case "13":
                        await controller.ViewCourses(admin);
                        break;
                    case "14":
                        await controller.ViewRooms(admin);
                        break;
                    case "15":
                        await controller.ViewEvents(admin);
                        break;
                    case "16":
                        await controller.ViewLogs(admin);
                        break;
                    case "17":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        /// <summary>
        /// Displays the users.
        /// </summary>
        /// <param name="people">The people object.</param>
        /// <param name="type">The type of users.</param>
        public static void PrintUsers(IEnumerable<Person> people, string type)
        {
            Console.WriteLine($"\n===== {type}s =====");
            int index = 1;
            foreach (var p in people)
            {
                Console.WriteLine($"{index++}. Name: {p.FirstName} {p.LastName}");
                Console.WriteLine($"    Username: {p.Username}");
            }
        }

        /// <summary>
        /// Displays the users.
        /// </summary>
        public static void CreateUserMenu()
        {
            Console.WriteLine("\n===== Create User =====");
            Console.WriteLine("1. Create a Student");
            Console.WriteLine("2. Create a Teacher");
            Console.WriteLine("3. Create an Admin");
            Console.WriteLine("4. Back");
        }

        public static void ConfirmRoomCreation(Room room)
        {
            Console.WriteLine($"\n===== Confirm {room.GetType().Name} =====");
            Console.WriteLine($"Room Number: {room.RoomNumber}");
            Console.WriteLine($"Seats: {room.NumberOfSeats}");

            switch (room)
            {
                case ConferenceRoom conf:
                    Console.WriteLine($"Projector: {(conf.HasProjector ? "Yes" : "No")}");
                    break;
                case ClassRoom classroom:
                    Console.WriteLine($"Projector: {(classroom.HasProjector ? "Yes" : "No")}");
                    Console.WriteLine($"Computers: {(classroom.HasComputers ? "Yes" : "No")}");
                    break;
                case Office office:
                    Console.WriteLine($"Shared Office: {(office.Shared ? "Yes" : "No")}");
                    break;
            }
        }

        /// <summary>
        /// Confirms the creation of a user.
        /// </summary>
        /// <param name="person">The person to confirm creation for.</param>
        public static void ConfirmUserCreation(Person person)
        {
            Console.WriteLine($"\n===== Confirm {person.GetType().Name} Creation =====");
            Console.WriteLine($"Username: {person.Username}");
            Console.WriteLine($"Password: {person.Password}");
            Console.WriteLine($"First Name: {person.FirstName}");
            Console.WriteLine($"Last Name: {person.LastName}");
            Console.WriteLine($"Age: {person.Age}");

            switch (person)
            {
                case Student s:
                    Console.WriteLine($"Student ID: {s.StudentID}");
                    Console.WriteLine($"Program: {s.Program}");
                    Console.WriteLine($"Year: {s.Year}");
                    break;

                case Teacher t:
                    Console.WriteLine($"Department: {t.Department}");
                    break;

                case Admin:
                    Console.WriteLine("Admin account.");
                    break;
            }
        }

        /// <summary>
        /// Displays the courses.
        /// </summary>
        /// <param name="courses">The courses object.</param>
        public static void PrintCourses(IEnumerable<Course> courses)
        {
            Console.WriteLine($"\n===== Courses =====");
            int index = 1;
            foreach (var c in courses)
            {
                Console.WriteLine($"{index++}. {c.Subject} ({c.CourseCode})");
                foreach (var s in c.Sections!)
                {
                    Console.WriteLine($"    Section: {s.SectionNumber} - Teacher: {s.Teacher!.FirstName} {s.Teacher.LastName}");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Confirms the creation of a course.
        /// </summary>
        /// <param name="course">The course object.</param>
        public static void ConfirmCourseCreation(Course course)
        {
            Console.WriteLine("\n===== Confirm Course Creation =====");
            Console.WriteLine($"Course Code: {course.CourseCode}");
            Console.WriteLine($"Subject: {course.Subject}");
            Console.WriteLine($"Teachers: {course.Teachers!.Count} (to be assigned later)");
            Console.WriteLine($"Sections: {course.Sections!.Count} (to be assigned later)");
        }

        /// <summary>
        /// Displays the rooms.
        /// </summary>
        /// <param name="rooms">The rooms object.</param>
        public static void PrintRooms(IEnumerable<Room> rooms)
        {
            Console.WriteLine($"\n===== Rooms =====");
            int index = 1;
            foreach (var r in rooms)
            {
                Console.WriteLine($"{index++}. Room: {r.RoomNumber} - Capacity: {r.NumberOfSeats}");
            }
        }

        public static void CreateRoomMenu()
        {
            Console.WriteLine("\n===== Create Room =====");
            Console.WriteLine("1. Create a Conference Room");
            Console.WriteLine("2. Create a Classroom");
            Console.WriteLine("3. Create an Office");
            Console.WriteLine("4. Back");
        }

        /// <summary>
        /// Displays the events.
        /// </summary>
        /// <param name="events">The events object.</param>
        public static void PrintEvents(IEnumerable<Event> events)
        {
            Console.WriteLine($"\n===== Events =====");
            int index = 1;
            foreach (var e in events)
            {
                Console.WriteLine($"{index++}. Event: {e.Title} ({e.StartDateTime})");
                Console.WriteLine($"    Location: {e.Room.RoomNumber} - Participants: {e.Participants.Count} / {e.MaxCapacity}");
            }
        }

        /// <summary>
        /// Displays the create event menu.
        /// </summary>
        public static void CreateEventMenu()
        {
            Console.WriteLine("\n===== Create Event =====");
            Console.WriteLine("1. Create a Conference");
            Console.WriteLine("2. Create an Office Meeting");
            Console.WriteLine("3. Back");
        }

        /// <summary>
        /// Confirms the creation of an event.
        /// </summary>
        /// <param name="ev">The event to confirm.</param>
        public static void ConfirmEventCreation(Event ev)
        {
            Console.WriteLine($"\n===== Confirm {ev.GetType().Name} =====");
            Console.WriteLine($"Title: {ev.Title}");
            Console.WriteLine($"Room: {ev.Room.RoomNumber}");
            Console.WriteLine($"Start: {ev.StartDateTime}");
            Console.WriteLine($"End: {ev.EndDateTime}");
            Console.WriteLine($"Recurring: {ev.IsRecurring}");
            Console.WriteLine($"Status: {ev.Status}");

            switch (ev)
            {
                case ClassSession cs:
                    Console.WriteLine($"Course: {cs.Course.Subject}");
                    Console.WriteLine($"Semester: {cs.Semester}, Section: {cs.Section}");
                    break;
                case Conference conf:
                    Console.WriteLine($"Speaker: {conf.Speaker!.FirstName} {conf.Speaker.LastName}");
                    break;
                case OfficeMeeting om:
                    Console.WriteLine($"Teacher: {om.Teacher.FirstName} {om.Teacher.LastName}");
                    break;
            }
        }

        /// <summary>
        /// Displays the admin profile.
        /// </summary>
        /// <param name="admin">The admin object.</param>
        private static void ShowProfile(Admin admin)
        {
            Console.WriteLine("\n===== Admin Profile =====");
            Console.WriteLine($"Name: {admin.FirstName} {admin.LastName}");
            Console.WriteLine($"Username: {admin.Username}");
            Console.WriteLine($"Age: {admin.Age}");
            Console.WriteLine($"Description: {admin.Description}");
            Console.WriteLine($"Last Login: {admin.LastLogin}");
            Console.WriteLine($"Created Users: {admin.CreatedUsersCount}");
        }

        /// <summary>
        /// Prints the admin logs.
        /// </summary>
        /// <param name="logs">The admin logs.</param>
        public static void PrintLogs(IEnumerable<AdminLog> logs)
        {
            Console.WriteLine("\n===== Admin Logs =====");
            foreach (var log in logs)
            {
                Console.WriteLine($"Date: {log.Timestamp}, Action: {log.Action}, Admin: {log.Admin.Username}");
            }
        }
    }
}
