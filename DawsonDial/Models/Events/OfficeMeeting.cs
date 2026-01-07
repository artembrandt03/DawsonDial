using System.Diagnostics.CodeAnalysis;
using DawsonDial.Models.Enums;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;

namespace DawsonDial.Models.Events
{
    /// <summary>
    /// Represents an office meeting.
    /// </summary>
    public class OfficeMeeting : Event
    {
        // Office Meeting specific properties
        public virtual Teacher Teacher { get; set; } = null!;

        [SetsRequiredMembers]
        protected OfficeMeeting() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfficeMeeting"/> class.
        /// </summary>
        /// <param name="title">The title of the meeting.</param>
        /// <param name="description">The description of the meeting.</param>
        /// <param name="room">The room where the meeting will take place.</param>
        /// <param name="startDateTime">The start date and time of the meeting.</param>
        /// <param name="endDateTime">The end date and time of the meeting.</param>
        /// <param name="isRecurring">Indicates whether the meeting is recurring.</param>
        /// <param name="status">The status of the meeting.</param>
        /// <param name="teacher">The teacher leading the meeting.</param>
        /// <exception cref="ArgumentNullException">Thrown when the teacher is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the duration is less than 15 minutes or greater than 1 hour.</exception>
        public OfficeMeeting(
        string title,
        string description,
        Room room,
        DateTime startDateTime,
        DateTime endDateTime,
        bool isRecurring,
        EventStatus status,
        Teacher teacher)
        : base(title, description, room, startDateTime, endDateTime, isRecurring, status)
        {
            TimeSpan duration = endDateTime - startDateTime;
            if (duration < TimeSpan.FromMinutes(15))
                throw new ArgumentException("Duration must be longer than 15 minutes.");
            if (duration > TimeSpan.FromHours(1))
                throw new ArgumentException("Duration must be shorter than 1 hour.");
            Teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));
        }

        // Office Meeting Methods
        /// <summary>
        /// Adds a student to the meeting.
        /// </summary>
        /// <param name="student">The student to add.</param>
        public void AddStudent(Student student) => AddParticipant(student);

        /// <summary>
        /// Removes a student from the meeting.
        /// </summary>
        /// <param name="student">The student to remove.</param>
        public void RemoveStudent(Student student) => RemoveParticipant(student);

        /// <summary>
        /// Checks if a student is in the meeting.
        /// </summary>
        /// <param name="student">The student to check.</param>
        /// <returns>True if the student is in the meeting, false otherwise.</returns>
        public bool HasStudent(Student student) => HasParticipant(student);
    }
}
