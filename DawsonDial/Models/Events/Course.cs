using System.Diagnostics.CodeAnalysis;
using DawsonDial.Repositories.Interfaces;
using DawsonDial.Models.People;

namespace DawsonDial.Models.Events
{
    /// <summary>
    /// Represents a course.
    /// </summary>
    public class Course : IAggregateRoot
    {
        // Course Properties
        public virtual Guid CourseId { get; set; } = Guid.NewGuid();
        public virtual string CourseCode { get; set; } = null!;
        public virtual string Subject { get; set; } = null!;
        public virtual HashSet<Section>? Sections { get; set; }
        public virtual HashSet<Teacher>? Teachers { get; set; }
        public byte[]? RowVersion { get; set; }

        [SetsRequiredMembers]
        protected Course() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Course"/> class.
        /// </summary>
        /// <param name="courseCode">The course code.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="teachers">The teachers.</param>
        /// <param name="sections">The sections.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the parameters are null.</exception>
        public Course(
            string courseCode,
            string subject,
            HashSet<Teacher> teachers,
            HashSet<Section> sections)
        {
            CourseCode = courseCode ?? throw new ArgumentNullException(nameof(courseCode));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Teachers = teachers ?? throw new ArgumentNullException(nameof(teachers));
            Sections = sections ?? throw new ArgumentNullException(nameof(sections));
        }
    }
}
