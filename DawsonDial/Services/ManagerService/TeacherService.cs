using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Repositories.Interfaces;

namespace DawsonDial.Services.ManagerService
{
    /// <summary>
    /// Provides services for managing teachers.
    /// </summary>
    public class TeacherService : UserService
    {
        // TeacherService backing fields
        private readonly IPersonRepository _personRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeacherService"/> class.
        /// </summary>
        public TeacherService(IPersonRepository personRepository) : base(personRepository)
        {
            // Initialize the teacher repository
            _personRepository = personRepository;
        }

        /// <summary>
        /// Assigns a course to a teacher.
        /// </summary>
        /// <param name="teacher">The teacher to assign the course to.</param>
        /// <param name="section">The section to assign the course to.</param>
        /// <exception cref="ArgumentNullException">Thrown if teacher or section is null.</exception>
        public async Task AddCourse(Teacher teacher, Section section)
        {
            ArgumentNullException.ThrowIfNull(teacher);
            ArgumentNullException.ThrowIfNull(section);

            if (!teacher.Classes.Any(c => c.SectionId == section.SectionId))
            {
                teacher.Classes.Add(section);
                await _personRepository.UpdateAsync(teacher);
            }
        }

        /// <summary>
        /// Gets the office hours for a teacher.
        /// </summary>
        /// <param name="teacher">The teacher to get the office hours for.</param>
        /// <returns>The office hours for the teacher.</returns>
        /// <exception cref="ArgumentNullException">Thrown if teacher is null.</exception>
        public OfficeHours GetOfficeHours(Teacher teacher)
        {
            ArgumentNullException.ThrowIfNull(teacher);

            return teacher.OfficeHours ?? throw new ArgumentNullException("Office hours not set.");
        }

        /// <summary>
        /// Drops a section from a teacher's schedule.
        /// </summary>
        /// <param name="teacher">The teacher to drop the section from.</param>
        /// <param name="section">The section to drop.</param>
        /// <exception cref="ArgumentNullException">Thrown if the section is not found.</exception>
        public async Task DeleteCourseSection(Teacher teacher, Section section)
        {
            Section sectionFound = teacher.Classes.FirstOrDefault(c => c.SectionId == section.SectionId) ?? throw new ArgumentNullException("Cannot remove nonexistent class.");
            teacher.Classes.Remove(sectionFound);

            await _personRepository.UpdateAsync(teacher);
        }

        /// <summary>
        /// Gets the schedule for a teacher.
        /// </summary>
        /// <param name="teacher">The teacher to get the schedule for.</param>
        /// <returns>The schedule for the teacher.</returns>
        public Schedule GetSchedule(Teacher teacher)
        {
            return default!;
        }

        /// <summary>
        /// Sets the office hours for a teacher.
        /// </summary>
        /// <param name="teacher">The teacher to set the office hours for</param>
        /// <param name="officeHours">The office hours to set</param>
        /// <returns>Async Task</returns>
        public async Task SetOfficeHoursAsync(Teacher teacher, OfficeHours officeHours)
        {
            await _personRepository.SetOfficeHoursAsync(teacher, officeHours);
        }

        /// <summary>
        /// Gets the teacher with their schedules.
        /// </summary>
        /// <param name="teacherId">The ID of the teacher to get.</param>
        /// <returns>The teacher with their schedules.</returns>
        /// <exception cref="ArgumentNullException">Thrown if teacher is null.</exception>
        public async Task<Teacher> GetTeacherWithSchedulesAsync(Guid teacherId)
        {
            return await _personRepository.GetTeacherWithSchedulesAsync(teacherId);
        }
    }
}
