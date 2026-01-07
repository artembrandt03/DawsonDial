using DawsonDial.Controllers;
using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;

namespace DawsonDial.Views
{
    /// <summary>
    /// Provides methods for displaying teacher menu views.
    /// </summary>
    public static class TeacherViews
    {
        /// <summary>
        /// Displays the teacher menu view.
        /// </summary>
        /// <param name="teacher">The teacher.</param>
        /// <param name="controller">The teacher controller.</param>
        /// <param name="authController">The authentication controller.</param>
        public static async Task ShowAsync(Teacher teacher, TeacherController controller, AuthController authController)
        {
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine($"\n===== Teacher Menu [{teacher.FirstName} {teacher.LastName}] =====");
                Console.WriteLine("1.  View Profile");
                Console.WriteLine("2.  Update Profile");
                Console.WriteLine("3.  Change Password");
                Console.WriteLine("4.  View My Courses");
                Console.WriteLine("5.  Set Office Hours");
                Console.WriteLine("6.  Schedule Event");
                Console.WriteLine("7.  Unregister From Event");
                Console.WriteLine("8.  View Room Availability");
                Console.WriteLine("9.  View My Events");
                Console.WriteLine("10. View My Schedule");
                Console.WriteLine("11. Logout");
                Console.Write("> ");

                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        ShowProfile(teacher);
                        break;
                    case "2":
                        await controller.UpdateProfileAsync(teacher);
                        break;
                    case "3":
                        await controller.ChangePasswordAsync(teacher);
                        break;
                    case "4":
                        controller.ViewSections(teacher);
                        break;
                    case "5":
                        await controller.SetOfficeHoursAsync(teacher);
                        break;
                    case "6":
                        await controller.ScheduleEventAsync(teacher);
                        break;
                    case "7":
                        await controller.UnregisterFromEventAsync(teacher);
                        break;
                    case "8":
                        await controller.ViewRoomAvailabilityAsync();
                        break;
                    case "9":
                        controller.ViewMyEvents(teacher);
                        break;
                    case "10":
                        controller.ViewSchedule(teacher);
                        break;
                    case "11":
                        authController.Logout(teacher);
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        /// <summary>
        /// Displays the teacher profile.
        /// </summary>
        /// <param name="teacher">The teacher.</param>
        private static void ShowProfile(Teacher teacher)
        {
            Console.WriteLine("\n===== Teacher Profile =====");
            Console.WriteLine($"Name: {teacher.FirstName} {teacher.LastName}");
            Console.WriteLine($"Username: {teacher.Username}");
            Console.WriteLine($"Age: {teacher.Age}");
            Console.WriteLine($"Description: {teacher.Description}");
            Console.WriteLine($"Department: {teacher.Department}");
            if (teacher.OfficeRoom != null)
            {
                Console.WriteLine($"Office Room: {teacher.OfficeRoom.RoomNumber}");
            }
        }

        /// <summary>
        /// Prints the sections for a teacher.
        /// </summary>
        /// <param name="sections">The sections to print.</param>
        public static void PrintSections(IEnumerable<Section> sections)
        {
            Console.WriteLine("\n===== Your Sections =====");

            int index = 1;
            foreach (var s in sections)
            {
                Console.WriteLine($"{index++}. {s.Course.Subject} ({s.Course.CourseCode})");
                Console.WriteLine($"   Section: {s.SectionNumber}");
                Console.WriteLine($"   Students: {s.EnrolledStudents.Count}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Prints the schedule for a teacher.
        /// </summary>
        /// <param name="schedule">The schedule to print.</param>
        /// <param name="teacher">The teacher whose schedule is being printed.</param>
        public static void PrintSchedule(Schedule schedule, Teacher teacher)
        {
            Console.WriteLine($"\n===== {teacher.FirstName}'s Schedule =====");
            Console.WriteLine($"From {schedule.StartDate:MM/dd/yyyy} to {schedule.EndDate:MM/dd/yyyy}");

            foreach (var slot in schedule.TimeSlots)
            {
                Console.WriteLine($"- {slot.DayOfWeek}: {slot.StartTime:hh\\:mm} - {slot.EndTime:hh\\:mm}");
            }
        }

        /// <summary>
        /// Prints the list of available events.
        /// </summary>
        /// <param name="events">The events to print.</param>
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
        /// Prints the list of available rooms.
        /// </summary>
        /// <param name="rooms">The rooms to print.</param>
        /// <param name="date">The date to print.</param>
        public static void PrintRoomAvailability(IEnumerable<Room> rooms, DateTime date)
        {
            Console.WriteLine($"\n===== Available Rooms on {date:MM/dd/yyyy} =====");

            var classrooms = rooms.OfType<ClassRoom>().ToList();
            var conferenceRooms = rooms.OfType<ConferenceRoom>().ToList();

            int index = 1;

            if (classrooms.Count > 0)
            {
                Console.WriteLine("\nClassrooms:");
                foreach (var room in classrooms)
                {
                    Console.WriteLine($"{index++}. Room {room.RoomNumber} - Seats: {room.NumberOfSeats}");
                    Console.WriteLine($"   Projector: {(room.HasProjector ? "Yes" : "No")}, Computers: {(room.HasComputers ? "Yes" : "No")}");
                }
            }

            if (conferenceRooms.Count > 0)
            {
                Console.WriteLine("\nConference Rooms:");
                foreach (var room in conferenceRooms)
                {
                    Console.WriteLine($"{index++}. Room {room.RoomNumber} - Seats: {room.NumberOfSeats}");
                    Console.WriteLine($"   Projector: {(room.HasProjector ? "Yes" : "No")}");
                }
            }
        }

        /// <summary>
        /// Prints a list of events for the teacher.
        /// </summary>
        /// <param name="events">The events to print.</param>
        public static void PrintMyEvents(IEnumerable<Event> events)
        {
            Console.WriteLine("\n===== Your Events =====");
            int index = 1;
            foreach (var e in events)
            {
                Console.WriteLine($"{index++}. {e.Title}");
                Console.WriteLine($"   Date: {e.StartDateTime:MM/dd/yyyy HH:mm} - {e.EndDateTime:HH:mm}");
                Console.WriteLine($"   Location: Room {e.Room.RoomNumber}");
                Console.WriteLine($"   Status: {e.Status}, Participants: {e.Participants.Count}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Prints a confirmation message for an office hour.
        /// </summary>
        /// <param name="office">The office hour to print.</param>
        public static void PrintOfficeHourConfirmation(OfficeHours office)
        {
            bool isDropIn = office.IsDropIn ?? false;
            Console.WriteLine("\n===== Office Hours Set =====");
            Console.WriteLine($"Title: {office.Title}");
            Console.WriteLine($"Location: {office.Room.RoomNumber}");
            Console.WriteLine($"From {office.StartDateTime:MM/dd/yyyy} to {office.EndDateTime:MM/dd/yyyy}");
            foreach (var slot in office.Schedule.TimeSlots)
            {
                Console.WriteLine($"- {slot.DayOfWeek}: {slot.StartTime:hh\\:mm} - {slot.EndTime:hh\\:mm}");
            }
            Console.WriteLine($"Drop-in: {(isDropIn ? "Yes" : "No")}");
        }

        /// <summary>
        /// Prints a confirmation message for an event.
        /// </summary>
        /// <param name="e">The event to print.</param>
        public static void PrintEventConfirmation(Event e)
        {
            Console.WriteLine("\n===== Event Scheduled =====");
            Console.WriteLine($"Title: {e.Title}");
            Console.WriteLine($"Date: {e.StartDateTime:MM/dd/yyyy}");
            Console.WriteLine($"Time: {e.StartDateTime:HH:mm} - {e.EndDateTime:HH:mm}");
            Console.WriteLine($"Room: {e.Room.RoomNumber}");
        }
    }
}
