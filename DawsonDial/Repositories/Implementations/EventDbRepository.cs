using DawsonDial.Models.Events;
using DawsonDial.Models.Contexts;
using DawsonDial.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DawsonDial.Repositories
{
    /// <summary>
    /// Represents a repository for managing events using Entity Framework Core.
    /// Implements the <see cref="IEventRepository"/> interface.
    /// </summary>
    public class EventDbRepository : IEventRepository
    {
        private readonly DawsonDialContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDbRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public EventDbRepository(DawsonDialContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all events from the database.
        /// </summary>
        /// <returns>A list of events.</returns>
        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _context.Events
                .Include(e => e.Room)
                .Include(e => e.Participants)
                .ToListAsync();
        }

        /// <summary>
        /// Gets an event by its ID from the database.
        /// </summary>
        /// <param name="id">The ID of the event.</param>
        /// <returns>The event with the specified ID.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the event with the specified ID is not found.</exception>
        public async Task<Event> GetByIdAsync(Guid id)
        {
            var eventEntity = await _context.Events
                .Include(e => e.Room)
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (eventEntity == null)
                throw new KeyNotFoundException($"Event with ID {id} not found");

            return eventEntity;
        }

        /// <summary>
        /// Adds a new event to the database.
        /// </summary>
        /// <param name="eventEntity">The event to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when the event is null.</exception>
        public async Task AddAsync(Event eventEntity)
        {
            if (eventEntity == null)
                throw new ArgumentNullException(nameof(eventEntity));

            await _context.Events.AddAsync(eventEntity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing event in the database.
        /// </summary>
        /// <param name="eventEntity">The event to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when the event is null.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the event with the specified ID is not found.</exception>
        public async Task UpdateAsync(Event eventEntity)
        {
            if (eventEntity == null)
                throw new ArgumentNullException(nameof(eventEntity));

            var existingEvent = await GetByIdAsync(eventEntity.EventId);
            if (existingEvent == null)
                throw new KeyNotFoundException($"Event with ID {eventEntity.EventId} not found");

            _context.Events.Update(eventEntity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an event from the database.
        /// </summary>
        /// <param name="id">The ID of the event to delete.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the event with the specified ID is not found.</exception>
        public async Task DeleteAsync(Guid id)
        {
            var eventEntity = await GetByIdAsync(id);

            if (eventEntity == null)
                throw new KeyNotFoundException($"Event with ID {id} not found");

            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();
        }
    }
}
