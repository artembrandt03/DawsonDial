using System;
using System.Collections.Generic;
using System.Linq;
using DawsonDial.Models.Events;
using DawsonDial.Repositories.Interfaces;

namespace DawsonDial.Services.ManagerService
{
    /// <summary>
    /// Provides functionality for managing schedules.
    /// </summary>
    public class ScheduleService
    {
        //Attributes
        private readonly IScheduleRepository _schedules;

        public IScheduleRepository Schedules => _schedules;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleService"/> class.
        /// </summary>
        /// <param name="schedules">The repository for schedules.</param>
        /// <exception cref="ArgumentNullException">Thrown if the repository is null.</exception>
        public ScheduleService(IScheduleRepository schedules)
        {
            _schedules = schedules ?? throw new ArgumentNullException(nameof(schedules));
        }


        /// <summary>
        /// Adds a new schedule.
        /// </summary>
        /// <param name="schedule">The schedule to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the schedule is null.</exception>
        public async Task AddSchedule(Schedule schedule)
        {
            ArgumentNullException.ThrowIfNull(schedule);
            await _schedules.AddAsync(schedule);
        }

        /// <summary>
        /// Removes a schedule.
        /// </summary>
        /// <param name="schedule">The schedule to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if the schedule is null.</exception>
        public async Task RemoveSchedule(Schedule schedule)
        {
            ArgumentNullException.ThrowIfNull(schedule);
            Guid id = schedule.ScheduleId;
            await _schedules.DeleteAsync(id);
        }

        /// <summary>
        /// Updates a schedule.
        /// </summary>
        /// <param name="updated">The updated schedule.</param>
        /// <exception cref="ArgumentNullException">Thrown if the updated schedule is null.</exception>
        public async Task UpdateSchedule(Schedule updated)
        {
            ArgumentNullException.ThrowIfNull(updated);
            await _schedules.UpdateAsync(updated);
        }

        /// <summary>
        /// Finds a schedule by its Guid.
        /// </summary>
        /// <param name="id">The Guid of the schedule to find.</param>
        /// <returns>The schedule with the given Guid.</returns>
        public async Task<Schedule> GetScheduleById(Guid id)
        {
            var schedule = await _schedules.GetByIdAsync(id);
            return schedule;
        }

        /// <summary>
        /// Returns all schedules.
        /// </summary>
        /// <returns>A set of all schedules.</returns>
        public async Task<IEnumerable<Schedule>> GetAllSchedules()
        {
            return await _schedules.GetAllAsync();
        }
    }
}
