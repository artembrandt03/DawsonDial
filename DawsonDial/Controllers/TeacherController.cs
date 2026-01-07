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
    /// Represents the teacher controller.
    /// </summary>
    public class TeacherController
    {
        private readonly DawsonDialService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeacherController"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public TeacherController(DawsonDialService service)
        {
            _service = service;
        }

        /// <summary>
        /// Updates the profile of a teacher.
        /// </summary>
        /// <param name="teacher">The teacher.</param>
        public async Task UpdateProfileAsync(Teacher teacher)
        {
            Console.WriteLine("\n===== Update Profile =====");
            Console.WriteLine("Leave blank to keep current value.");

            string firstName = InputPrompter.Prompt($"First Name [{teacher.FirstName}]: ");
            if (!string.IsNullOrWhiteSpace(firstName)) teacher.FirstName = firstName;

            string lastName = InputPrompter.Prompt($"Last Name [{teacher.LastName}]: ");
            if (!string.IsNullOrWhiteSpace(lastName)) teacher.LastName = lastName;

            string ageStr = InputPrompter.Prompt($"Age [{teacher.Age}]: ");
            if (int.TryParse(ageStr, out int age)) teacher.Age = age;

            string desc = InputPrompter.Prompt($"Description [{teacher.Description}]: ");
            if (!string.IsNullOrWhiteSpace(desc)) teacher.Description = desc;

            try
            {
                await _service.UpdateUserProfile(teacher);
                Console.WriteLine("\nProfile updated successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError updating profile: {e.Message}");
            }
        }

        /// <summary>
        /// Changes the password for a teacher.
        /// </summary>
        /// <param name="teacher">The teacher whose password is to be changed.</param>
        public async Task ChangePasswordAsync(Teacher teacher)
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
                await _service.ChangeUserPassword(teacher.Username, current, newPass);
                Console.WriteLine("\nPassword updated successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError changing password: {e.Message}");
            }
        }

        /// <summary>
        /// Views the teacher's sections.
        /// </summary>
        /// <param name="teacher">The teacher whose sections are to be viewed.</param>
        public void ViewSections(Teacher teacher)
        {
            try
            {
                var sections = teacher.Classes;
                TeacherViews.PrintSections(sections);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError retrieving sections: {e.Message}");
            }
        }

        /// <summary>
        /// Views the schedule of a teacher.
        /// </summary>
        /// <param name="teacher">The teacher whose schedule is to be viewed.</param>
        public void ViewSchedule(Teacher teacher)
        {
            try
            {
                var schedule = _service.GetScheduleByTeacher(teacher);
                TeacherViews.PrintSchedule(schedule, teacher);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError retrieving schedule: {e.Message}");
            }
        }

        /// <summary>
        /// Views the events of a teacher.
        /// </summary>
        /// <param name="teacher">The teacher whose events are to be viewed.</param>
        public void ViewMyEvents(Teacher teacher)
        {
            try
            {
                var events = teacher.Events
                    .Where(e => e.StartDateTime > DateTime.Now && e.Status != EventStatus.Cancelled)
                    .OrderBy(e => e.StartDateTime)
                    .ToList();

                TeacherViews.PrintEventList(events);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError retrieving events: {e.Message}");
            }
        }

        /// Sets the office hours of a teacher.
        /// </summary>
        /// <param name="teacher">The teacher whose office hours are to be set.</param>
        public async Task SetOfficeHoursAsync(Teacher teacher)
        {
            Console.WriteLine("\n===== Set Office Hours =====");
            var timeSlots = new Dictionary<DayOfWeek, (TimeOnly Start, TimeOnly End)>();

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                Console.WriteLine($"\n{day}");

                bool response = InputPrompter.Confirm($"Include hours for {day}?");
                if (!response) continue;

                TimeOnly startTime = InputPrompter.PromptTime("Start Time (HH:MM): ");
                TimeOnly endTime = InputPrompter.PromptTime("End Time (HH:MM): ");

                if (startTime >= endTime)
                {
                    Console.WriteLine("Start time must be before end time. Skipping.");
                    continue;
                }

                timeSlots[day] = (startTime, endTime);
            }

            if (timeSlots.Count == 0)
            {
                Console.WriteLine("No time slots provided.");
                return;
            }

            DateOnly startDate = InputPrompter.PromptDate("Start Date (MM/DD/YYYY): ");
            DateOnly endDate = InputPrompter.PromptDate("End Date (MM/DD/YYYY): ", min: startDate);

            bool isDropIn = InputPrompter.Confirm("Are these drop-in office hours?");

            var schedule = new Schedule(timeSlots, startDate, endDate);
            var startDateTime = startDate.ToDateTime(timeSlots.First().Value.Start);
            var endDateTime = endDate.ToDateTime(timeSlots.First().Value.End);
            var office = teacher.OfficeRoom!;

            var officeHours = new OfficeHours(
                $"{teacher.FirstName} {teacher.LastName}'s Office Hours",
                $"Office hours for {teacher.FirstName} in {office.RoomNumber}",
                office,
                startDateTime,
                endDateTime,
                true,
                EventStatus.Confirmed,
                teacher,
                isDropIn,
                schedule
            );

            try
            {
                await _service.SetTeacherOfficeHours(teacher, officeHours);
                TeacherViews.PrintOfficeHourConfirmation(officeHours);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Schedules a new event for the teacher.
        /// </summary>
        /// <param name="teacher">The teacher scheduling the event.</param>
        public async Task ScheduleEventAsync(Teacher teacher)
        {
            Console.WriteLine("\n===== Schedule New Event =====");

            string title = InputPrompter.Prompt("Event Title: ");
            string description = InputPrompter.Prompt("Event Description: ");

            DateOnly date = InputPrompter.PromptDate("Date (MM/DD/YYYY): ");
            TimeOnly startTime = InputPrompter.PromptTime("Start Time (HH:MM): ");
            int durationMinutes = InputPrompter.PromptInt("Duration in minutes: ", 1);

            DateTime startDateTime = date.ToDateTime(startTime);
            DateTime endDateTime = startDateTime.AddMinutes(durationMinutes);
            TimeSpan duration = endDateTime - startDateTime;

            if (startDateTime < DateTime.Now)
            {
                Console.WriteLine("Cannot schedule an event in the past.");
                return;
            }

            var availableRooms = await _service.GetAvailableRoomsAt(startDateTime);
            if (availableRooms.Count == 0)
            {
                Console.WriteLine("No available rooms at the specified time.");
                return;
            }

            TeacherViews.PrintRoomAvailability(availableRooms, startDateTime);

            int roomChoice = InputPrompter.PromptInt("Select a room (0 to cancel): ", 0, availableRooms.Count);
            if (roomChoice == 0) return;

            Room selectedRoom = availableRooms.ElementAt(roomChoice - 1);

            Console.WriteLine("\nChoose Event Type:");
            int typeChoice = InputPrompter.PromptInt("\n1. Conference\n2. Meeting\nSelect type: ", 1, 2);
            Event scheduledEvent;

            try
            {
                scheduledEvent = typeChoice switch
                {
                    1 => new Conference(
                        title,
                        description,
                        selectedRoom,
                        startDateTime,
                        endDateTime,
                        false,
                        EventStatus.Planned,
                        teacher),
                    2 => new OfficeMeeting(
                        title,
                        description,
                        selectedRoom,
                        startDateTime,
                        endDateTime,
                        false,
                        EventStatus.Planned,
                        teacher),
                    _ => throw new ArgumentException("Invalid event type selected.")
                };

                scheduledEvent.AddParticipant(teacher);

                bool reserved = await _service.ReserveRoom(selectedRoom, scheduledEvent, startDateTime, duration);
                if (!reserved)
                {
                    Console.WriteLine("Room could not be reserved. Try a different time or room.");
                    return;
                }

                await _service.ScheduleEvent(scheduledEvent);
                teacher.AddEvent(scheduledEvent);

                TeacherViews.PrintEventConfirmation(scheduledEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError scheduling event: {ex.Message}");
            }
        }

        /// <summary>
        /// Unregisters a teacher from an upcoming event.
        /// </summary>
        /// <param name="teacher">The teacher to unregister.</param>
        public async Task UnregisterFromEventAsync(Teacher teacher)
        {
            Console.WriteLine("\n===== Unregister from Event =====");

            var upcomingEvents = teacher.Events
                .Where(e => e.StartDateTime > DateTime.Now && e.Status != EventStatus.Cancelled)
                .OrderBy(e => e.StartDateTime)
                .ToList();

            if (upcomingEvents.Count == 0)
            {
                Console.WriteLine("You have no upcoming events to unregister from.");
                return;
            }

            TeacherViews.PrintEventList(upcomingEvents);

            int choice = InputPrompter.PromptInt("Select an event to unregister (0 to cancel): ", 0, upcomingEvents.Count);
            if (choice == 0) return;

            Event selectedEvent = upcomingEvents[choice - 1];
            bool confirm = InputPrompter.Confirm($"Are you sure you want to unregister from '{selectedEvent.Title}'?");
            if (!confirm) return;

            try
            {
                await _service.UnregisterUserFromEvent(teacher, selectedEvent);
                teacher.RemoveEvent(selectedEvent);
                Console.WriteLine($"Successfully unregistered from '{selectedEvent.Title}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Displays the availability of rooms for a given date.
        /// </summary>
        public async Task ViewRoomAvailabilityAsync()
        {
            Console.WriteLine("\n===== View Room Availability =====");

            string input = InputPrompter.Prompt("Date to check (MM/DD/YYYY, or leave blank for today): ");
            DateTime dateToCheck;

            if (string.IsNullOrWhiteSpace(input))
            {
                dateToCheck = DateTime.Now.AddMinutes(1);
            }
            else if (!DateTime.TryParse(input, out dateToCheck))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            try
            {
                var rooms = await _service.GetAvailableRoomsAt(dateToCheck);
                if (rooms.Count == 0)
                {
                    Console.WriteLine("No rooms are available at that time.");
                    return;
                }

                TeacherViews.PrintRoomAvailability(rooms, dateToCheck);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving room availability: {ex.Message}");
            }
        }
    }
}
