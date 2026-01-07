using DawsonDial.Controllers;
using DawsonDial.Models.Events;
using DawsonDial.Helpers;
using DawsonDial.Models.People;

namespace DawsonDial.Views
{
    /// <summary>
    /// Provides methods for displaying the student menu views.
    /// </summary>
    public static class StudentViews
    {
        /// <summary>
        /// Displays the student menu and handles user input.
        /// </summary>
        /// <param name="student">The student object.</param>
        /// <param name="controller">The student controller.</param>
        /// <param name="authController">The authentication controller.</param>
        public static async Task ShowAsync(Student student, StudentController controller, AuthController authController)
        {
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine($"\n===== Student Menu [{student.FirstName} {student.LastName}] =====");
                Console.WriteLine("1.  View Profile");
                Console.WriteLine("2.  Update Profile");
                Console.WriteLine("3.  Change Password");
                Console.WriteLine("4.  View Available Courses");
                Console.WriteLine("5.  View My Courses");
                Console.WriteLine("6.  Enroll In Course");
                Console.WriteLine("7.  Drop Course");
                Console.WriteLine("8.  View Available Events");
                Console.WriteLine("9.  Register For Event");
                Console.WriteLine("10. Unregister From Event");
                Console.WriteLine("11. Book Teacher Meeting");
                Console.WriteLine("12. View My Schedule");
                Console.WriteLine("13. Logout");
                Console.Write("> ");

                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        ShowProfile(student);
                        break;
                    case "2":
                        await controller.UpdateProfileAsync(student);
                        break;
                    case "3":
                        await controller.ChangePasswordAsync(student);
                        break;
                    case "4":
                        await controller.ViewAvailableCoursesAsync();
                        break;
                    case "5":
                        controller.ViewMySections(student);
                        break;
                    case "6":
                        await controller.EnrollInCourseAsync(student);
                        break;
                    case "7":
                        await controller.DropSectionAsync(student);
                        break;
                    case "8":
                        await controller.ViewAvailableEventsAsync();
                        break;
                    case "9":
                        await controller.RegisterForEventAsync(student);
                        break;
                    case "10":
                        await controller.UnregisterFromEventAsync(student);
                        break;
                    case "11":
                        await controller.BookTeacherMeetingAsync(student);
                        break;
                    case "12":
                        controller.ViewSchedule(student);
                        break;
                    case "13":
                        authController.Logout(student);
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        /// <summary>
        /// Displays the available courses.
        /// </summary>
        /// <param name="courses">The collection of courses.</param>
        public static void PrintAvailableCourses(IEnumerable<Course> courses)
        {
            Console.WriteLine("\n===== Available Courses =====");
            int i = 1;
            foreach (var c in courses)
            {
                Console.WriteLine($"{i++}. {c.Subject} ({c.CourseCode})");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Displays the enrolled sections.
        /// </summary>
        /// <param name="sections">The collection of sections.</param>
        public static void PrintEnrolledSections(IEnumerable<Section> sections)
        {
            Console.WriteLine("\n===== Enrolled Sections =====");
            int index = 1;
            foreach (var s in sections)
            {
                Console.WriteLine($"{index++}. {s.Course.Subject} - Section {s.SectionNumber}");
                Console.WriteLine($"   Teacher: {s.Teacher?.FirstName} {s.Teacher?.LastName}");
            }
        }

        /// <summary>
        /// Displays the sections.
        /// </summary>
        /// <param name="sections">The collection of sections.</param>
        public static void PrintSections(IEnumerable<Section> sections)
        {
            Console.WriteLine("\n===== Available Sections =====");
            int index = 1;
            foreach (var s in sections)
            {
                Console.WriteLine($"{index++}. {s.Course.Subject} - Section {s.SectionNumber}");
                Console.WriteLine($"   Teacher: {s.Teacher?.FirstName} {s.Teacher?.LastName}");
            }
        }

        /// <summary>
        /// Displays a list of available events.
        /// </summary>
        /// <param name="events">The list of events.</param>
        public static void PrintEventList(IEnumerable<Event> events)
        {
            Console.WriteLine("\n===== Available Events =====");
            int index = 1;
            foreach (var e in events)
            {
                Console.WriteLine($"{index++}. {e.Title}");
                Console.WriteLine($"   {e.StartDateTime:MM/dd/yyyy HH:mm} - {e.EndDateTime:HH:mm}");
                Console.WriteLine($"   Room: {e.Room.RoomNumber}, Seats: {e.AvailableSeats}/{e.MaxCapacity}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Prints the student's schedule.
        /// </summary>
        /// <param name="schedule">The schedule object.</param>
        /// <param name="student">The student object.</param>
        public static void PrintSchedule(Schedule schedule, Student student)
        {
            Console.WriteLine($"\n===== {student.FirstName}'s Schedule =====");
            Console.WriteLine($"From {schedule.StartDate:MM/dd/yyyy} to {schedule.EndDate:MM/dd/yyyy}");

            foreach (var slot in schedule.TimeSlots)
            {
                Console.WriteLine($"- {slot.DayOfWeek}: {slot.StartTime:hh\\:mm} - {slot.EndTime:hh\\:mm}");
            }
        }

        /// <summary>
        /// Prints the list of teachers with their office hours.
        /// </summary>
        /// <param name="teachers">The list of teachers.</param>
        public static void PrintTeacherListWithOfficeHours(IEnumerable<Teacher> teachers)
        {
            Console.WriteLine("\n===== Your Teachers =====");

            int index = 1;
            foreach (var t in teachers)
            {
                Console.WriteLine($"{index++}. {t.FirstName} {t.LastName} - {t.Department}");

                if (t.OfficeRoom != null)
                    Console.WriteLine($"   Office: {t.OfficeRoom.RoomNumber}");

                if (t.OfficeHours != null)
                {
                    Console.WriteLine("   Office Hours:");
                    foreach (var slot in t.OfficeHours.Schedule.TimeSlots)
                    {
                        Console.WriteLine($"     {slot.DayOfWeek}: {slot.StartTime:hh\\:mm} - {slot.EndTime:hh\\:mm}");
                    }
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Prints the confirmation of a meeting.
        /// </summary>
        /// <param name="meeting">The meeting object.</param>
        /// <param name="teacher">The teacher object.</param>
        public static void PrintMeetingConfirmation(OfficeMeeting meeting, Teacher teacher)
        {
            Console.WriteLine("\n===== Meeting Booked Successfully =====");
            Console.WriteLine($"With: {teacher.FirstName} {teacher.LastName}");
            Console.WriteLine($"Date: {meeting.StartDateTime:MM/dd/yyyy}");
            Console.WriteLine($"Time: {meeting.StartDateTime:HH:mm} - {meeting.EndDateTime:HH:mm}");
            Console.WriteLine($"Location: Room {meeting.Room.RoomNumber}");
            Console.WriteLine($"Topic: {meeting.Title}");
        }

        /// <summary>
        /// Displays the student profile.
        /// </summary>
        /// <param name="student">The student object.</param>
        private static void ShowProfile(Student student)
        {
            Console.WriteLine("\n===== Student Profile =====");
            Console.WriteLine($"Name: {student.FirstName} {student.LastName}");
            Console.WriteLine($"Username: {student.Username}");
            Console.WriteLine($"Age: {student.Age}");
            Console.WriteLine($"Description: {student.Description}");
            Console.WriteLine($"Student ID: {student.StudentID}");
            Console.WriteLine($"Program: {student.Program}");
            Console.WriteLine($"Year: {student.Year}");
        }
    }
}
