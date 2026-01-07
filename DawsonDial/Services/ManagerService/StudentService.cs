using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Repositories.Interfaces;

namespace DawsonDial.Services.ManagerService
{
    /// <summary>
    /// Provides services for managing student-related operations.
    /// </summary>
    public class StudentService : UserService
    {
        // StudentService backing fields
        private readonly IPersonRepository _personRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentService"/> class.
        /// </summary>
        public StudentService(IPersonRepository personRepository) : base(personRepository)
        {
            // Initialize the teacher repository
            _personRepository = personRepository;
        }

        /// <summary>
        /// Enrolls a student in a course section.
        /// </summary>
        /// <param name="student">The student to enroll.</param>
        /// <param name="section">The course section to enroll in.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the student or section is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the student is already enrolled in the course section.</exception>
        public async Task EnrollInCourse(Student student, Section section)
        {
            ArgumentNullException.ThrowIfNull(student);
            ArgumentNullException.ThrowIfNull(section);

            if (student.Classes.Any(c => c.SectionId == section.SectionId))
            {
                throw new InvalidOperationException("Student is already enrolled in this class.");
            }

            student.AddClass(section);

            await _personRepository.UpdateAsync(student);
        }

        /// <summary>
        /// Drops a student from a course section.
        /// </summary>
        /// <param name="student">The student to drop.</param>
        /// <param name="section">The course section to drop from.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the student or section is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the student is not enrolled in the course section.</exception>
        public async Task DropCourse(Student student, Section section)
        {
            ArgumentNullException.ThrowIfNull(student);
            ArgumentNullException.ThrowIfNull(section);

            if (!student.Classes.Contains(section))
            {
                throw new InvalidOperationException("Student is not enrolled in this section.");
            }

            student.RemoveClass(section);

            await _personRepository.UpdateAsync(student);
        }

        /// <summary>
        /// Gets the course sections a student is enrolled in.
        /// </summary>
        /// <param name="student">The student to get the enrolled courses for.</param>
        /// <returns>A set of course sections the student is enrolled in.</returns>
        public HashSet<Section> GetEnrolledCourses(Student student)
        {
            return student.Classes;
        }

        public async Task<Student> GetStudentWithSchedulesAsync(Guid studentId)
        {
            ArgumentNullException.ThrowIfNull(studentId);

            return await _personRepository.GetStudentWithSchedulesAsync(studentId);
        }
    }
}
