using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Models.Contexts;
using Microsoft.EntityFrameworkCore;
using DawsonDial.Repositories.Interfaces;
using DawsonDial.Security;

namespace DawsonDial.Repositories
{

    /// <summary>
    /// Represents a repository for managing people using Entity Framework Core.
    /// Implements the <see cref="IPersonRepository"/> interface.
    /// </summary>
    public class PersonDbRepository : IPersonRepository
    {
        private readonly DawsonDialContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonDbRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public PersonDbRepository(DawsonDialContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all people from the database.
        /// </summary>
        /// <returns>A list of people.</returns>
        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _context.People.ToListAsync();
        }

        /// <summary>
        /// Gets a person by their ID from the database.
        /// </summary>
        /// <param name="id">The ID of the person.</param>
        /// <returns>The person with the specified ID.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the person with the specified ID is not found.</exception>
        public async Task<Person> GetByIdAsync(Guid id)
        {
            return await _context.People.FindAsync(id) ?? throw new KeyNotFoundException($"Person with ID {id} not found.");
        }

        /// <summary>
        /// Adds a new person to the database.
        /// </summary>
        /// <param name="entity">The person to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when the person is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the person could not be added to the database.</exception>
        /// <exception cref="DbUpdateException">Thrown when there is an error while saving changes to the database.</exception>
        public async Task AddAsync(Person entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity), "Person cannot be null.");

            // Hash The Password
            entity.Password = PasswordHasher.Hash(entity.Password);

            using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead);
            try
            {
                if (entity.GetType() == typeof(Teacher))
                {
                    var teacher = (Teacher)entity;
                    await _context.Teachers.AddAsync(teacher);
                }
                else if (entity.GetType() == typeof(Student))
                {
                    var student = (Student)entity;
                    await _context.Students.AddAsync(student);
                }
                else if (entity.GetType() == typeof(Admin))
                {
                    var admin = (Admin)entity;
                    await _context.Admins.AddAsync(admin);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Failed to add person to the database.", ex);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }

        /// <summary>
        /// Updates an existing person in the database.
        /// </summary>
        /// <param name="entity">The person to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when the person is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the person could not be updated in the database.</exception>
        /// <exception cref="DbUpdateException">Thrown when there is an error while saving changes to the database.</exception>
        public async Task UpdateAsync(Person entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity), "Person cannot be null.");
            using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead);

            try
            {
                _context.People.Update(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Failed to update person in the database.", ex);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }

        /// <summary>
        /// Sets the office hours for a teacher.
        /// </summary>
        /// <param name="teacherInput">The teacher to add the office hours to</param>
        /// <param name="newOfficeHours">The new office hours to set</param>
        /// <returns>Async Task</returns>
        /// <exception cref="InvalidOperationException">If teacher or office hours are null.</exception>
        public async Task SetOfficeHoursAsync(Teacher teacherInput, OfficeHours newOfficeHours)
        {
            ArgumentNullException.ThrowIfNull(teacherInput);
            ArgumentNullException.ThrowIfNull(newOfficeHours);

            var trackedTeacher = await _context.Teachers
                .Include(t => t.OfficeHours)
                .Include(t => t.OfficeRoom)
                .FirstOrDefaultAsync(t => t.PersonId == teacherInput.PersonId);

            if (trackedTeacher is null)
                throw new InvalidOperationException("Teacher not found.");

            // Delete old OfficeHours if exists
            if (trackedTeacher.OfficeHours is not null)
            {
                _context.OfficeHours.Remove(trackedTeacher.OfficeHours);
                await _context.SaveChangesAsync();
            }

            // Add Schedule if new
            var schedule = newOfficeHours.Schedule;
            if (schedule != null && !_context.Schedules.Any(s => s.ScheduleId == schedule.ScheduleId))
            {
                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();
            }

            // Set relationships
            newOfficeHours.TeacherId = trackedTeacher.PersonId;
            newOfficeHours.Teacher = trackedTeacher;

            trackedTeacher.OfficeHours = newOfficeHours;
            _context.OfficeHours.Add(newOfficeHours);

            await _context.SaveChangesAsync();
        }



        /// <summary>
        /// Deletes a person from the database by their ID.
        /// </summary>
        /// <param name="id">The ID of the person to delete.</param>
        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            _context.People.Remove(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a person by their username from the database.
        /// </summary>
        /// <param name="username">The username of the person.</param>
        /// <returns>The person with the specified username.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the person with the specified username is not found.</exception>
        public async Task<Person> GetUserByUsernameAsync(string username)
        {
            var person = await _context.People.FirstOrDefaultAsync(p => p.Username == username);

            if (person is Teacher teacher)
            {
                await _context.Entry(teacher)
                    .Collection(t => t.Classes)
                    .Query()
                    .Include(s => s.Course)
                    .Include(s => s.EnrolledStudents)
                    .LoadAsync();

                await _context.Entry(teacher)
                    .Reference(t => t.OfficeRoom)
                    .LoadAsync();

                await _context.Entry(teacher)
                    .Reference(t => t.OfficeHours)
                    .LoadAsync();
            }
            else if (person is Student student)
            {
                await _context.Entry(student)
                    .Collection(s => s.Classes)
                    .Query()
                    .Include(s => s.Course)
                    .Include(s => s.Teacher!)
                        .ThenInclude(t => t!.OfficeRoom!)
                    .Include(s => s.Teacher!)
                        .ThenInclude(t => t!.OfficeHours!)
                    .LoadAsync();
            }

            return person!;
        }

        /// <summary>
        /// Gets a person by their username and password from the database.
        /// </summary>
        /// <param name="username">The username of the person.</param>
        /// <param name="password">The password of the person.</param>
        /// <returns>The person with the specified username and password.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the person with the specified username is not found.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the password is invalid.</exception>
        public async Task<Person> GetUserByUsernameAndPasswordAsync(string username, string password)
        {
            var person = await GetUserByUsernameAsync(username) ?? throw new KeyNotFoundException($"User with username {username} not found.");

            if (!PasswordHasher.Verify(password, person.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            return person;
        }

        /// <summary>
        /// Updates a person's password in the database by their username.
        /// /// </summary>
        /// <param name="username">The username of the person.</param>
        /// <param name="password">The new password of the person.</param>
        /// <returns>The updated person.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the person with the specified username is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the password could not be updated in the database.</exception>
        /// <exception cref="DbUpdateException">Thrown when there is an error while saving changes to the database.</exception>
        public async Task<Person> UpdateUserPasswordByUsernameAsync(string username, string password)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead);
            try
            {
                var person = await GetUserByUsernameAsync(username) ?? throw new KeyNotFoundException($"User with username {username} not found.");

                // Hash The Password
                person.Password = PasswordHasher.Hash(password);

                _context.People.Update(person);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return person;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Failed to update user password.", ex);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }

        /// <summary>
        /// Gets a teacher by their ID, including all related schedules and time slots.
        /// </summary>
        public async Task<Teacher> GetTeacherWithSchedulesAsync(Guid teacherId)
        {
            return await _context.Teachers
                .Include(t => t.Classes)
                    .ThenInclude(s => s.Schedule)
                        .ThenInclude(schedule => schedule.TimeSlots)
                .Include(t => t.OfficeHours)
                    .ThenInclude(oh => oh!.Schedule)
                        .ThenInclude(schedule => schedule.TimeSlots)
                .FirstOrDefaultAsync(t => t.PersonId == teacherId)
                ?? throw new KeyNotFoundException($"Teacher with ID {teacherId} not found.");
        }

        /// <summary>
        /// Gets a student by their ID, including all related schedules and time slots.
        /// </summary>
        /// <param name="studentId">The ID of the student.</param>
        /// <returns>The student with the specified ID.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the student with the specified ID is not found.</exception>
        
        public async Task<Student> GetStudentWithSchedulesAsync(Guid studentId)
        {
            return await _context.Students
                .Include(s => s.Classes)
                    .ThenInclude(c => c.Schedule)
                        .ThenInclude(schedule => schedule.TimeSlots)
                .FirstOrDefaultAsync(s => s.PersonId == studentId)
                ?? throw new KeyNotFoundException($"Student with ID {studentId} not found.");
        }
    }
}
