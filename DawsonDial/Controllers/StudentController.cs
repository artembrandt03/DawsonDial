using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.Enums;
using DawsonDial.Helpers;
using DawsonDial.Views;
using DawsonDial.Services.ManagerService;

namespace DawsonDial.Controllers
{
    public class StudentController
    {
        private readonly DawsonDialService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentController"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public StudentController(DawsonDialService service)
        {
            _service = service;
        }

        /// <summary>
        /// Updates the profile of a student.
        /// </summary>
        /// <param name="student">The student.</param>
        public async Task UpdateProfileAsync(Student student)
        {
            Console.WriteLine("\n===== Update Profile =====");
            Console.WriteLine("Leave blank to keep current value.");

            string firstName = InputPrompter.Prompt($"First Name [{student.FirstName}]: ");
            if (!string.IsNullOrWhiteSpace(firstName)) student.FirstName = firstName;

            string lastName = InputPrompter.Prompt($"Last Name [{student.LastName}]: ");
            if (!string.IsNullOrWhiteSpace(lastName)) student.LastName = lastName;

            string ageStr = InputPrompter.Prompt($"Age [{student.Age}]: ");
            if (int.TryParse(ageStr, out int age)) student.Age = age;

            string desc = InputPrompter.Prompt($"Description [{student.Description}]: ");
            if (!string.IsNullOrWhiteSpace(desc)) student.Description = desc;

            try
            {
                await _service.UpdateUserProfile(student);
                Console.WriteLine("\nProfile updated successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError updating profile: {e.Message}");
            }
        }

        /// <summary>
        /// Changes the password for the given student.
        /// </summary>
        /// <param name="student">The student whose password is to be changed.</param>
        public async Task ChangePasswordAsync(Student student)
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
                await _service.ChangeUserPassword(student.Username, current, newPass);
                Console.WriteLine("\nPassword updated successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError changing password: {e.Message}");
            }
        }

        /// <summary>
        /// Displays the available courses for the given student.
        /// </summary>
        public async Task ViewAvailableCoursesAsync()
        {
            try
            {
                var courses = await _service.GetAvailableCourses();

                if (courses.Count == 0)
                {
                    Console.WriteLine("\nNo courses available.");
                    return;
                }

                StudentViews.PrintAvailableCourses(courses);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError retrieving courses: {e.Message}");
            }
        }

        /// <summary>
        /// Displays the sections the given student is enrolled in.
        /// </summary>
        /// <param name="student">The student whose sections are to be displayed.</param>
        public void ViewMySections(Student student)
        {
            try
            {
                StudentViews.PrintEnrolledSections(student.Classes);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError retrieving courses: {e.Message}");
            }
        }

        /// <summary>
        /// Enrolls the given student in a course.
        /// </summary>
        /// <param name="student">The student to be enrolled.</param>
        public async Task EnrollInCourseAsync(Student student)
        {
            try
            {
                var courses = await _service.GetAvailableCourses();
                if (courses.Count == 0)
                {
                    Console.WriteLine("\nNo courses available for enrollment.");
                    return;
                }

                StudentViews.PrintAvailableCourses(courses);
                int courseIndex = InputPrompter.PromptInt("Enter course number (0 to cancel): ", 0, courses.Count);
                if (courseIndex == 0) return;
                var course = courses.ElementAt(courseIndex - 1);

                var sections = course.Sections!.ToList();
                StudentViews.PrintSections(sections);

                int sectionIndex = InputPrompter.PromptInt("Select section number (0 to cancel): ", 0, sections.Count);
                if (sectionIndex == 0) return;
                var section = sections[sectionIndex - 1];

                await _service.EnrollStudentInCourse(student, section);
                Console.WriteLine($"\nSuccessfully enrolled in {course.Subject}, section {section.SectionNumber}.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nEnrollment failed: {e.Message}");
            }
        }

        /// <summary>
        /// Drops the given student from a course section.
        /// </summary>
        /// <param name="student">The student to drop the section from.</param>
        public async Task DropSectionAsync(Student student)
        {
            try
            {
                StudentViews.PrintEnrolledSections(student.Classes);
                int dropIndex = InputPrompter.PromptInt("Select section number to drop (0 to cancel): ", 0, student.Classes.Count);
                if (dropIndex == 0) return;
                var section = student.Classes.ElementAt(dropIndex - 1);
                if (section == null) return;

                await _service.DropStudentFromCourse(student, section);
                Console.WriteLine($"\nDropped course {section.Course.Subject}, section {section.SectionNumber}.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError dropping course: {e.Message}");
            }
        }

        /// <summary>
        /// Displays a list of available events.
        /// </summary>
        public async Task ViewAvailableEventsAsync()
        {
            try
            {
                var events = await _service.GetAvailableEvents();
                if (events.Count == 0)
                {
                    Console.WriteLine("\nNo available events.");
                    return;
                }

                StudentViews.PrintEventList(events);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError loading events: {e.Message}");
            }
        }

        /// <summary>
        /// Registers a student for an event.
        /// </summary>
        /// <param name="student">The student to register.</param>
        public async Task RegisterForEventAsync(Student student)
        {
            try
            {
                var events = await _service.GetAvailableEvents();
                if (events.Count == 0)
                {
                    Console.WriteLine("\nNo events available to register for.");
                    return;
                }

                StudentViews.PrintEventList(events);
                int eventIndex = InputPrompter.PromptInt("Enter event number (0 to cancel): ", 0, events.Count);
                if (eventIndex == 0) return;
                var selectedEvent = events.ElementAt(eventIndex - 1);
                if (selectedEvent == null) return;

                await _service.RegisterUserForEvent(student, selectedEvent);
                student.AddEvent(selectedEvent);
                Console.WriteLine($"\nRegistered for event: {selectedEvent.Title}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError registering for event: {e.Message}");
            }
        }

        /// <summary>
        /// Unregisters a student from an event.
        /// </summary>
        /// <param name="student">The student to unregister.</param>
        public async Task UnregisterFromEventAsync(Student student)
        {
            try
            {
                var futureEvents = student.Events
                    .Where(e => e.Status != EventStatus.Cancelled && e.StartDateTime > DateTime.Now)
                    .OrderBy(e => e.StartDateTime)
                    .ToList();

                if (futureEvents.Count == 0)
                {
                    Console.WriteLine("\nYou have no active events to unregister from.");
                    return;
                }

                StudentViews.PrintEventList(futureEvents);
                int eventIndex = InputPrompter.PromptInt("Select event to unregister (0 to cancel): ", 0, futureEvents.Count);
                if (eventIndex == 0) return;

                string confirm = InputPrompter.Prompt("Are you sure? (y/n): ").ToLower();
                if (confirm != "y" && confirm != "yes") return;

                var selectedEvent = futureEvents.ElementAt(eventIndex - 1);
                if (selectedEvent == null) return;

                await _service.UnregisterUserFromEvent(student, selectedEvent);
                student.RemoveEvent(selectedEvent);
                Console.WriteLine($"\nSuccessfully unregistered from: {selectedEvent.Title}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError unregistering: {e.Message}");
            }
        }

        /// <summary>
        /// Books a meeting with a teacher.
        /// </summary>
        /// <param name="student">The student booking the meeting.</param>
        public async Task BookTeacherMeetingAsync(Student student)
        {
            try
            {
                if (student.Classes is null || student.Classes.Count == 0)
                {
                    Console.WriteLine("\nYou are not enrolled in any courses.");
                    return;
                }

                HashSet<Teacher> teachers = student.Classes
                    .Select(s => s.Teacher!)
                    .ToHashSet();

                if (teachers.Count == 0)
                {
                    Console.WriteLine("\nNo teachers found.");
                    return;
                }

                StudentViews.PrintTeacherListWithOfficeHours(teachers);

                int selection = InputPrompter.PromptInt("Select a teacher (0 to cancel): ", 0, teachers.Count);
                if (selection == 0) return;

                Teacher selectedTeacher = teachers.ElementAt(selection - 1);

                Console.WriteLine($"\nScheduling a meeting with {selectedTeacher.FirstName} {selectedTeacher.LastName}");

                string topic = InputPrompter.Prompt("Meeting Topic: ");
                string description = InputPrompter.Prompt("Meeting Description: ");

                DateOnly date = InputPrompter.PromptDate("Meeting Date (MM/DD/YYYY): ", min: DateOnly.FromDateTime(DateTime.Now));
                TimeOnly startTime = InputPrompter.PromptTime("Meeting Start Time (HH:MM): ");
                int durationMinutes = InputPrompter.PromptInt("Meeting Duration (15–60 min): ", 15, 60);

                TimeOnly endTime = startTime.AddMinutes(durationMinutes);
                DateTime startDateTime = date.ToDateTime(startTime);
                DateTime endDateTime = date.ToDateTime(endTime);

                Room office = selectedTeacher.OfficeRoom!;
                var meeting = new OfficeMeeting(
                    topic,
                    description,
                    office,
                    startDateTime,
                    endDateTime,
                    false,
                    EventStatus.Planned,
                    selectedTeacher);

                meeting.AddParticipant(student);
                meeting.AddParticipant(selectedTeacher);

                bool reserved = await _service.ReserveRoom(office, meeting, startDateTime, TimeSpan.FromMinutes(durationMinutes));

                if (reserved)
                {
                    await _service.ScheduleEvent(meeting);
                    student.AddEvent(meeting);
                    selectedTeacher.AddEvent(meeting);

                    StudentViews.PrintMeetingConfirmation(meeting, selectedTeacher);
                }
                else
                {
                    Console.WriteLine("\nFailed to book the room. It may no longer be available.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError booking meeting: {e.Message}");
            }
        }

        /// <summary>
        /// Displays the schedule for a student.
        /// </summary>
        /// <param name="student">The student whose schedule to display.</param>
        public void ViewSchedule(Student student)
        {
            try
            {
                var schedule = _service.GetScheduleByStudent(student);
                StudentViews.PrintSchedule(schedule, student);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nFailed to display schedule: {e.Message}");
            }
        }
    }
}
