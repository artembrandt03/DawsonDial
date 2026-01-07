using DawsonDial.Models.Contexts;
using DawsonDial.Models.Rooms;
using DawsonDial.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DawsonDial.Repositories
{
    /// <summary>
    /// Represents a repository for managing Room entities using Entity Framework Core.
    /// Implements the <see cref="IRoomRepository"/> interface.
    /// </summary>
    public class RoomDbRepository : IRoomRepository
    {
        private readonly DawsonDialContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomDbRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public RoomDbRepository(DawsonDialContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all rooms from the database.
        /// </summary>
        /// <returns>A list of all rooms.</returns>
        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _context.Rooms.ToListAsync();
        }

        /// <summary>
        /// Retrieves a room by its unique identifier, including subclass-specific data if applicable.
        /// </summary>
        /// <param name="id">The ID of the room to retrieve.</param>
        /// <returns>The room with the specified ID.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when no room with the specified ID exists.</exception>
        public async Task<Room> GetByIdAsync(Guid id)
        {
            var room = await _context.Rooms
                .Include(r => (r as Office)!.AssignedTeachers)
                .Include(r => (r as ConferenceRoom)!.ScheduledEvents)
                .Include(r => (r as ClassRoom)!.AssignedClasses)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found");

            return room;
        }

        /// <summary>
        /// Adds a new room to the database.
        /// </summary>
        /// <param name="entity">The room to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the room is null.</exception>
        public async Task AddAsync(Room entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await _context.Rooms.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing room in the database.
        /// </summary>
        /// <param name="entity">The updated room entity.</param>
        /// <exception cref="ArgumentNullException">Thrown if the room is null.</exception>
        public async Task UpdateAsync(Room entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            _context.Rooms.Update(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a room from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the room to delete.</param>
        /// <exception cref="KeyNotFoundException">Thrown if the room with the given ID is not found.</exception>
        public async Task DeleteAsync(Guid id)
        {
            var room = await GetByIdAsync(id);
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }
    }
}