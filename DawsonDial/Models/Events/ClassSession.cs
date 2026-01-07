using System.Diagnostics.CodeAnalysis;
using DawsonDial.Models.Enums;
using DawsonDial.Models.Rooms;

namespace DawsonDial.Models.Events
{
    /// <summary>
    /// Represents a class session event.
    /// </summary>
    public class ClassSession : Event
    {
        // ClassSession Properties
        public virtual Course Course { get; set; } = null!;
        public virtual string Semester { get; set; } = null!;
        public virtual string Section { get; set; } = null!;
        public virtual Schedule Schedule { get; set; } = null!;

        [SetsRequiredMembers]
        protected ClassSession() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassSession"/> class.
        /// </summary>
        /// <param name="title">The title of the class session.</param>
        /// <param name="description">The description of the class session.</param>
        /// <param name="room">The room where the class session will take place.</param>
        /// <param name="course">The course associated with the class session.</param>
        /// <param name="semester">The semester of the class session.</param>
        /// <param name="section">The section of the class session.</param>
        /// <param name="startDateTime">The start date and time of the class session.</param>
        /// <param name="endDateTime">The end date and time of the class session.</param>
        /// <param name="isRecurring">Indicates whether the class session is recurring.</param>
        /// <param name="status">The status of the class session.</param>
        /// <param name="schedule">The schedule of the class session.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the required parameters are null.</exception>
        /// <exception cref="ArgumentException">Thrown if the semester or section is null or whitespace.</exception>
        public ClassSession(
            string title,
            string description,
            Room room,
            Course course,
            string semester,
            string section,
            DateTime startDateTime,
            DateTime endDateTime,
            bool isRecurring,
            EventStatus status,
            Schedule schedule)
            : base(title, description, room, startDateTime, endDateTime, isRecurring, status)
        {
            if (string.IsNullOrWhiteSpace(semester))
                throw new ArgumentException("Semester cannot be empty.", nameof(semester));
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentException("Section cannot be empty.", nameof(section));
            Course = course ?? throw new ArgumentNullException(nameof(course));
            Semester = semester ?? throw new ArgumentNullException(nameof(semester));
            Section = section ?? throw new ArgumentNullException(nameof(section));
            Schedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
        }

        // ClassSession Methods
        /// <summary>
        /// Returns a string representation of the class session.
        /// </summary>
        /// <returns>A string representation of the class session.</returns>
        public override string ToString() =>
        $"{Course.CourseCode} - {Title} ({Semester} - {Section})";
    }
}
