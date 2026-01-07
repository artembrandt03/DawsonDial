using System.Diagnostics.CodeAnalysis;
using DawsonDial.Models.Enums;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;

namespace DawsonDial.Models.Events
{
    /// <summary>
    /// Represents an office hours event.
    /// </summary>
    public class OfficeHours : Event
    {
        public virtual Guid TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; } = null!;
        public virtual bool? IsDropIn { get; set; }
        public Guid ScheduleId { get; set; }
        public virtual Schedule Schedule { get; set; } = null!;

        [SetsRequiredMembers]
        protected OfficeHours() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfficeHours"/> class.
        /// </summary>
        /// <param name="title">The title of the office hours.</param>
        /// <param name="description">The description of the office hours.</param>
        /// <param name="room">The room where the office hours will be held.</param>
        /// <param name="startDateTime">The start date and time of the office hours.</param>
        /// <param name="endDateTime">The end date and time of the office hours.</param>
        /// <param name="isRecurring">Indicates whether the office hours are recurring.</param>
        /// <param name="status">The status of the office hours.</param>
        /// <param name="teacher">The teacher associated with the office hours.</param>
        /// <param name="isDropIn">Indicates whether the office hours are drop-in.</param>
        /// <param name="schedule">The schedule of the office hours.</param>
        /// <exception cref="ArgumentException">Thrown if endDateTime is before startDateTime.</exception>
        /// <exception cref="ArgumentException">Thrown if isRecurring is true and schedule is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown if teacher is null.</exception>
        public OfficeHours(
            string title,
            string description,
            Room room,
            DateTime startDateTime,
            DateTime endDateTime,
            bool isRecurring,
            EventStatus status,
            Teacher teacher,
            bool isDropIn = false,
            Schedule? schedule = null)
            : base(title, description, room, startDateTime, endDateTime, isRecurring, status)
        {
            if (endDateTime < startDateTime)
                throw new ArgumentException("End date time must be after start date time.");

            if (isRecurring && schedule == null)
                throw new ArgumentException("Schedule cannot be null for recurring office hours.");

            Teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));
            IsDropIn = isDropIn;
            Schedule = schedule!;
        }
    }
}
