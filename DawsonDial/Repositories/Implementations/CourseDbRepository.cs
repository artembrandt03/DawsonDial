using DawsonDial.Models.Events;
using DawsonDial.Models.Contexts;
using DawsonDial.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DawsonDial.Repositories
{
    /// <summary>
    /// Represents a repository for managing courses using Entity Framework Core.
    /// Implements the <see cref="ICourseRepository"/> interface.
    /// </summary>
    public class CourseDbRepository : ICourseRepository
    {
        private readonly DawsonDialContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseDbRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public CourseDbRepository(DawsonDialContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all courses from the database.
        /// </summary>
        /// <returns>A list of courses.</returns>
        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Include(c => c.Sections)
                .Include(c => c.Teachers)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a course by its ID from the database.
        /// </summary>
        /// <param name="id">The ID of the course.</param>
        /// <returns>The course with the specified ID.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the course with the specified ID is not found.</exception>
        public async Task<Course> GetByIdAsync(Guid id)
        {
            var courseEntity = await _context.Courses
                .Include(c => c.Sections)
                .Include(c => c.Teachers)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (courseEntity == null)
                throw new KeyNotFoundException($"Course with ID {id} not found.");

            return courseEntity;
        }

        /// <summary>
        /// Adds a new course to the database.
        /// </summary>
        /// <param name="courseEntity">The course to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when the course is null.</exception>
        public async Task AddAsync(Course courseEntity)
        {
            if (courseEntity == null)
                throw new ArgumentNullException(nameof(courseEntity));

            await _context.Courses.AddAsync(courseEntity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing course in the database.
        /// </summary>
        /// <param name="courseEntity">The course to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when the course is null.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the course with the specified ID is not found.</exception>
        public async Task UpdateAsync(Course courseEntity)
        {
            if (courseEntity == null)
                throw new ArgumentNullException(nameof(courseEntity));

            var existingCourse = await GetByIdAsync(courseEntity.CourseId);
            if (existingCourse == null)
                throw new KeyNotFoundException($"Course with ID {courseEntity.CourseId} not found");

            _context.Courses.Update(courseEntity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a course from the database.
        /// </summary>
        /// <param name="id">The ID of the course to delete.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the course with the specified ID is not found.</exception>
        public async Task DeleteAsync(Guid id)
        {
            var courseEntity = await GetByIdAsync(id);

            if (courseEntity == null)
                throw new KeyNotFoundException($"Course with ID {id} not found");

            _context.Courses.Remove(courseEntity);
            await _context.SaveChangesAsync();
        }
    }
}
