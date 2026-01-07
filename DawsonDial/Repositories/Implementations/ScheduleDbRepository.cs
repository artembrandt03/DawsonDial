using DawsonDial.Models.Events;
using DawsonDial.Models.Contexts;
using DawsonDial.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DawsonDial.Repositories
{
    /// <summary>
    /// Represents a repository for managing schedules using Entity Framework Core.
    /// Implements the <see cref="IScheduleRepository"/> interface.
    /// </summary>
    public class ScheduleDbRepository : IScheduleRepository
    {
        private readonly DawsonDialContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleDbRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public ScheduleDbRepository(DawsonDialContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all schedules from the database.
        /// </summary>
        /// <returns>A list of schedules.</returns>
        public async Task<IEnumerable<Schedule>> GetAllAsync()
        {
            return await _context.Schedules
                .Include(s => s.TimeSlots)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a schedule by its ID from the database.
        /// </summary>
        /// <param name="id">The ID of the schedule.</param>
        /// <returns>The schedule with the specified ID.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the schedule with the specified ID is not found.</exception>
        public async Task<Schedule> GetByIdAsync(Guid id)
        {
            var scheduleEntity = await _context.Schedules
                .Include(s => s.TimeSlots)
                .FirstOrDefaultAsync(s => s.ScheduleId == id);

            if (scheduleEntity == null)
                throw new KeyNotFoundException($"Schedule with ID {id} not found");

            return scheduleEntity;
        }

        /// <summary>
        /// Adds a new schedule to the database.
        /// </summary>
        /// <param name="scheduleEntity">The schedule to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when the schedule is null.</exception>
        public async Task AddAsync(Schedule scheduleEntity)
        {
            if (scheduleEntity == null)
                throw new ArgumentNullException(nameof(scheduleEntity));

            await _context.Schedules.AddAsync(scheduleEntity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing schedule in the database.
        /// </summary>
        /// <param name="scheduleEntity">The schedule to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when the schedule is null.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the schedule with the specified ID is not found.</exception>
        public async Task UpdateAsync(Schedule scheduleEntity)
        {
            if (scheduleEntity == null)
                throw new ArgumentNullException(nameof(scheduleEntity));

            var existingSchedule = await GetByIdAsync(scheduleEntity.ScheduleId);
            if (existingSchedule == null)
                throw new KeyNotFoundException($"Schedule with ID {scheduleEntity.ScheduleId} not found");

            _context.Schedules.Update(scheduleEntity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a schedule from the database.
        /// </summary>
        /// <param name="id">The ID of the schedule to delete.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the schedule with the specified ID is not found.</exception>
        public async Task DeleteAsync(Guid id)
        {
            var scheduleEntity = await GetByIdAsync(id);

            if (scheduleEntity == null)
                throw new KeyNotFoundException($"Schedule with ID {id} not found");

            _context.Schedules.Remove(scheduleEntity);
            await _context.SaveChangesAsync();
        }
    }
}
