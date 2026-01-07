using System.Diagnostics.CodeAnalysis;
using DawsonDial.Repositories.Interfaces;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;

namespace DawsonDial.Models.Events
{
    /// <summary>
    /// Represents a section of a course.
    /// </summary>
    public class Section : IAggregateRoot
    {
        // Section properties
        public virtual Guid SectionId { get; set; } = Guid.NewGuid();
        public virtual Course Course { get; set; } = null!;
        public virtual int SectionNumber { get; set; }
        public virtual Teacher? Teacher { get; set; }
        public virtual Schedule Schedule { get; set; } = null!;
        public virtual HashSet<Student> EnrolledStudents { get; set; } = new HashSet<Student>();
        public virtual HashSet<ClassSession> ClassSessions { get; set; } = new HashSet<ClassSession>();
        public virtual ICollection<SectionRoom> Rooms { get; set; } = new List<SectionRoom>();
        public byte[]? RowVersion { get; set; }

        [SetsRequiredMembers]
        protected Section() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Section"/> class.
        /// </summary>
        /// <param name="course">The course associated with the section.</param>
        /// <param name="sectionNumber">The section number.</param>
        /// <param name="teacher">The teacher assigned to the section.</param>
        /// <param name="schedule">The schedule for the section.</param>
        /// <param name="rooms">The rooms available for the section.</param>
        /// <exception cref="ArgumentNullException">Thrown when the rooms are null.</exception>
        public Section(Course course,
                        int sectionNumber,
                        Teacher teacher,
                        Schedule schedule,
                        Dictionary<string, Room> rooms)
        {
            Course = course;
            SectionNumber = sectionNumber;
            Teacher = teacher;
            Schedule = schedule;

            if (rooms == null)
                throw new ArgumentNullException(nameof(rooms));

            foreach (var (key, room) in rooms)
            {
                Rooms.Add(new SectionRoom
                {
                    RoomKey = key,
                    Room = room
                });
            }
        }

        // Section Methods
        /// <summary>
        /// Enrolls a student in the section.
        /// </summary>
        /// <param name="student">The student to enroll.</param>
        /// <exception cref="ArgumentNullException">Thrown when the student is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the student is already enrolled.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the section is full.</exception>
        public void EnrollStudent(Student student)
        {
            // TODO: Add more validation to ensure student is valid before adding.
            ArgumentNullException.ThrowIfNull(student);
            if (IsStudentEnrolled(student))
                throw new InvalidOperationException("Student is already registered for this course.");

            if (IsSectionFull())
                throw new InvalidOperationException("Section is full.");

            EnrolledStudents.Add(student);
        }

        /// <summary>
        /// Drops a student from the section.
        /// </summary>
        /// <param name="student">The student to drop.</param>
        /// <exception cref="ArgumentNullException">Thrown when the student is null.</exception>
        public void DropStudent(Student student)
        {
            ArgumentNullException.ThrowIfNull(student);
            EnrolledStudents.Remove(student);
        }

        /// <summary>
        /// Checks if a student is enrolled in the section.
        /// </summary>
        /// <param name="student">The student to check.</param>
        /// <exception cref="ArgumentNullException">Thrown when the student is null.</exception>
        public bool IsStudentEnrolled(Student student)
        {
            ArgumentNullException.ThrowIfNull(student);
            return EnrolledStudents.Contains(student);
        }

        /// <summary>
        /// Adds a class session to the section.
        /// </summary>
        /// <param name="session">The session to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when the session is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the session is already scheduled for this course.</exception>
        public void AddClassSession(ClassSession session)
        {
            // TODO: Add more validation to ensure session is valid before adding.
            ArgumentNullException.ThrowIfNull(session);
            if (ClassSessions.Contains(session))
            {
                throw new InvalidOperationException("Session is already scheduled for this course.");
            }
            ClassSessions.Add(session);
        }

        /// <summary>
        /// Removes a class session from the section.
        /// </summary>
        /// <param name="session">The session to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown when the session is null.</exception>
        public void RemoveClassSession(ClassSession session)
        {
            ArgumentNullException.ThrowIfNull(session);
            ClassSessions.Remove(session);
        }

        /// <summary>
        /// Gets the schedule of class sessions for the section.
        /// </summary>
        /// <returns>A hash set of class sessions.</returns>
        public HashSet<ClassSession> GetSchedule() => ClassSessions;

        /// <summary>
        /// Checks if the section is full.
        /// </summary>
        /// <returns>True if the count of enrolled students is more or equal to the capcity of the smallest section room.</returns>
        private bool IsSectionFull()
        {
            if (!Rooms.Any())
                throw new InvalidOperationException("No rooms available for this section.");

            int capacity = Rooms.Min(sr => sr.Room.NumberOfSeats);
            return EnrolledStudents.Count >= capacity;
        }
    }
}
