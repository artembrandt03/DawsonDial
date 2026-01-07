using DawsonDial.Models.Events;
using DawsonDial.Models.Contexts;
using DawsonDial.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DawsonDial.Repositories
{
    /// <summary>
    /// Represents a repository for managing sections using Entity Framework Core.
    /// Implements the <see cref="ISectionRepository"/> interface.
    /// </summary>
    public class SectionDbRepository : ISectionRepository
    {
        private readonly DawsonDialContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionDbRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public SectionDbRepository(DawsonDialContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all sections from the database.
        /// </summary>
        /// <returns>A list of sections.</returns>
        public async Task<IEnumerable<Section>> GetAllAsync()
        {
            return await _context.Sections
            .Include(s => s.Schedule)
                .ThenInclude(schedule => schedule.TimeSlots)
            .Include(s => s.Course)
            .Include(s => s.Teacher)
            .Include(s => s.EnrolledStudents)
            .Include(s => s.Rooms)
                .ThenInclude(sr => sr.Room)
            .Include(s => s.ClassSessions)
            .ToListAsync();
        }

        /// <summary>
        /// Gets a section by its ID from the database.
        /// </summary>
        /// <param name="id">The ID of the section.</param>
        /// <returns>The section with the specified ID.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the section with the specified ID is not found.</exception>
        public async Task<Section> GetByIdAsync(Guid id)
        {
            var sectionEntity = await _context.Sections
                .Include(s => s.Schedule)
                    .ThenInclude(schedule => schedule.TimeSlots)
                .Include(s => s.Course)
                .Include(s => s.Teacher)
                .Include(s => s.EnrolledStudents)
                .Include(s => s.Rooms)
                    .ThenInclude(sr => sr.Room)
                .Include(s => s.ClassSessions)
                .FirstOrDefaultAsync(s => s.SectionId == id);

            if (sectionEntity == null)
                throw new KeyNotFoundException($"Section with ID {id} not found.");

            return sectionEntity;
        }

        /// <summary>
        /// Adds a new section to the database.
        /// </summary>
        /// <param name="sectionEntity">The section to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when the section is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the section's schedule is invalid.</exception>
        public async Task AddAsync(Section sectionEntity)
        {
            if (sectionEntity == null)
                throw new ArgumentNullException(nameof(sectionEntity));

            if (sectionEntity.Schedule != null)
            {
                if (sectionEntity.Schedule.StartDate > sectionEntity.Schedule.EndDate)
                    throw new ArgumentException("Schedule start date must be earlier than end date.");

                if (!sectionEntity.Schedule.TimeSlots.Any())
                    throw new ArgumentException("Schedule must have at least one time slot.");

                foreach (var timeSlot in sectionEntity.Schedule.TimeSlots)
                {
                    if (timeSlot.StartTime >= timeSlot.EndTime)
                        throw new ArgumentException($"Invalid time slot: Start time must be earlier than end time for {timeSlot.DayOfWeek}.");
                }
            }

            await _context.Sections.AddAsync(sectionEntity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing section in the database.
        /// </summary>
        /// <param name="sectionEntity">The section to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when the section is null.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the section with the specified ID is not found.</exception>
        public async Task UpdateAsync(Section sectionEntity)
        {
            if (sectionEntity == null)
                throw new ArgumentNullException(nameof(sectionEntity));

            var existingSection = await _context.Sections
                .Include(s => s.Schedule)
                    .ThenInclude(schedule => schedule.TimeSlots)
                .Include(s => s.Rooms)
                .Include(s => s.ClassSessions)
                .FirstOrDefaultAsync(s => s.SectionId == sectionEntity.SectionId);


            if (existingSection == null)
                throw new KeyNotFoundException($"Section with ID {sectionEntity.SectionId} not found.");

            // Update schedule if it exists
            if (existingSection.Schedule != null)
            {
                // Update existing schedule
                _context.TimeSlots.RemoveRange(existingSection.Schedule.TimeSlots);
                existingSection.Schedule.TimeSlots = sectionEntity.Schedule.TimeSlots;
                existingSection.Schedule.StartDate = sectionEntity.Schedule.StartDate;
                existingSection.Schedule.EndDate = sectionEntity.Schedule.EndDate;
            }
            else
            {
                // Create new schedule
                existingSection.Schedule = sectionEntity.Schedule;
            }

            // Update section rooms
            _context.SectionRooms.RemoveRange(existingSection.Rooms);
            existingSection.Rooms = sectionEntity.Rooms;

            // Update basic properties
            _context.Entry(existingSection).CurrentValues.SetValues(sectionEntity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a section from the database.
        /// </summary>
        /// <param name="id">The ID of the section to delete.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the section with the specified ID is not found.</exception>
        public async Task DeleteAsync(Guid id)
        {
            var existingSection = await _context.Sections
                .Include(s => s.Schedule)
                    .ThenInclude(schedule => schedule.TimeSlots)
                .Include(s => s.Rooms)
                .Include(s => s.ClassSessions)
                .FirstOrDefaultAsync(s => s.SectionId == id);

            if (existingSection == null)
                throw new KeyNotFoundException($"Section with ID {id} not found.");

            // Remove related entities
            if (existingSection.Schedule != null)
            {
                _context.TimeSlots.RemoveRange(existingSection.Schedule.TimeSlots);
                _context.Schedules.Remove(existingSection.Schedule);
            }

            _context.SectionRooms.RemoveRange(existingSection.Rooms);
            _context.ClassSessions.RemoveRange(existingSection.ClassSessions);

            // Remove the section
            _context.Sections.Remove(existingSection);
            await _context.SaveChangesAsync();
        }
    }
}
