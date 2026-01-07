using System;
using System.Collections.Generic;
using System.Linq;
using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Repositories.Interfaces;

namespace DawsonDial.Services.ManagerService
{
    /// <summary>
    /// This is a manager class for courses.
    /// </summary>
    public class CourseService
    {
        //Attributes
        private readonly ICourseRepository _courses;
        private readonly ISectionRepository _sections;

        //Property to expose internal courses (if needed externally in a unit test for example)
        public ICourseRepository Courses => _courses;
        public ISectionRepository Sections => _sections;

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseService"/> class.
        /// </summary>
        /// <param name="courses">The repository for courses.</param>
        /// <param name="sections">The repository for sections.</param>
        /// <exception cref="ArgumentNullException">Thrown when the courses repository or sections repository is null.</exception>
        public CourseService(ICourseRepository courses, ISectionRepository sections)
        {
            _courses = courses ?? throw new ArgumentNullException(nameof(courses));
            _sections = sections ?? throw new ArgumentNullException(nameof(sections));
        }

        /// <summary>
        /// Adds a new course.
        /// </summary>
        /// <param name="course">The course to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when the course is null.</exception>
        public async Task AddCourse(Course course)
        {
            ArgumentNullException.ThrowIfNull(course);
            await _courses.AddAsync(course);
        }

        /// <summary>
        /// Removes a course.
        /// </summary>
        /// <param name="course">The course to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown when the course is null.</exception>
        public async Task RemoveCourse(Course course)
        {
            ArgumentNullException.ThrowIfNull(course);
            await _courses.DeleteAsync(course.CourseId);
        }

        /// <summary>
        /// Updates a course.
        /// </summary>
        /// <param name="updated">The updated course.</param>
        /// <exception cref="ArgumentNullException">Thrown when the updated course is null.</exception>
        public async Task UpdateCourse(Course updated)
        {
            ArgumentNullException.ThrowIfNull(updated);
            await _courses.UpdateAsync(updated);
        }

        /// <summary>
        /// Adds a section.
        /// </summary>
        /// <param name="section">The section to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when the section is null.</exception>
        public async Task AddSection(Section section)
        {
            ArgumentNullException.ThrowIfNull(section);
            await _sections.AddAsync(section);
        }

        /// <summary>
        /// Removes a section.
        /// </summary>
        /// <param name="section">The section to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown when the section is null.</exception>
        public async Task RemoveSection(Section section)
        {
            ArgumentNullException.ThrowIfNull(section);
            await _sections.DeleteAsync(section.SectionId);
        }

        /// <summary>
        /// Updates a section.
        /// </summary>
        /// <param name="updated">The updated section.</param>
        /// <exception cref="ArgumentNullException">Thrown when the updated section is null.</exception>
        public async Task UpdateSection(Section updated)
        {
            ArgumentNullException.ThrowIfNull(updated);
            await _sections.UpdateAsync(updated);
        }

        /// <summary>
        /// Enrolls a student in a section.
        /// </summary>
        /// <param name="section">The section to enroll in.</param>
        /// <param name="student">The student to enroll.</param>
        /// <exception cref="ArgumentNullException">Thrown when the section or student is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the section does not exist.</exception>
        public async Task EnrollStudent(Section section, Student student)
        {
            ArgumentNullException.ThrowIfNull(section);
            ArgumentNullException.ThrowIfNull(student);

            var existingSection = await _sections.GetByIdAsync(section.SectionId);
            if (existingSection == null)
                throw new ArgumentException("Section not found");

            section.EnrollStudent(student);
            await _sections.UpdateAsync(section);
        }

        /// <summary>
        /// Unenrolls a student from a section.
        /// </summary>
        /// <param name="section">The section to unenroll from.</param>
        /// <param name="student">The student to unenroll.</param>
        /// <exception cref="ArgumentNullException">Thrown when the section or student is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the section does not exist.</exception>
        public async Task UnenrollStudent(Section section, Student student)
        {
            ArgumentNullException.ThrowIfNull(section);
            ArgumentNullException.ThrowIfNull(student);

            var existingSection = await _sections.GetByIdAsync(section.SectionId);
            if (existingSection == null)
                throw new ArgumentException("Section not found");

            section.DropStudent(student);
            await _sections.UpdateAsync(section);
        }

        /// <summary>
        /// Gets a course by ID.
        /// </summary>
        /// <param name="id">The ID of the course to get.</param>
        /// <returns>The course with the given ID.</returns>
        public async Task<Course> GetCourseById(Guid id)
        {
            return await _courses.GetByIdAsync(id);
        }

        /// <summary>
        /// Gets a section by ID.
        /// </summary>
        /// <param name="id">The ID of the section to get.</param>
        /// <returns>The section with the given ID.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the section with the given ID is not found.</exception>
        public async Task<Section> GetSectionById(Guid id)
        {
            return await _sections.GetByIdAsync(id);
        }

        /// <summary>
        /// Gets all courses.
        /// </summary>
        /// <returns>A set of all courses.</returns>
        public async Task<IEnumerable<Course>> GetAllCourses()
        {
            return await _courses.GetAllAsync();
        }

        /// <summary>
        /// Gets all sections.
        /// </summary>
        /// <returns>A set of all sections.</returns>
        public async Task<IEnumerable<Section>> GetAllSections()
        {
            return await _sections.GetAllAsync();
        }
    }
}
