using DawsonDial.Models.People;
using DawsonDial.Repositories.Interfaces;
using DawsonDial.Models.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DawsonDial.Repositories
{
    /// <summary>
    /// Repository for managing admin logs in the database.
    /// </summary>
    public class AdminLogDbRepository : IAdminLogRepository
    {
        /// <summary>
        /// The database context for accessing admin logs.
        /// </summary>
        private readonly DawsonDialContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminLogDbRepository"/> class.
        /// </summary>
        public AdminLogDbRepository(DawsonDialContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all admin logs from the database.
        /// </summary>
        public async Task<IEnumerable<AdminLog>> GetAllAsync()
        {
            return await _context.AdminLogs
            .Include(log => log.Admin) // This will load the related Admin entity
            .ToListAsync();
        }

        /// <summary>
        /// Gets an admin log by its ID.
        /// </summary>
        public async Task<AdminLog> GetByIdAsync(Guid id)
        {
            return await _context.AdminLogs.FindAsync(id)
                   ?? throw new KeyNotFoundException("Log not found.");
        }

        /// <summary>
        /// Adds a new admin log to the database.
        /// </summary>
        public async Task AddAsync(AdminLog log)
        {
            _context.AdminLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing admin log in the database.
        /// </summary>
        public async Task UpdateAsync(AdminLog log)
        {
            _context.AdminLogs.Update(log);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an admin log from the database.
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            var log = await GetByIdAsync(id);
            _context.AdminLogs.Remove(log);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all logs created by a specific admin.
        /// </summary>
        public async Task<IEnumerable<AdminLog>> GetLogsByAdminIdAsync(Guid adminId)
        {
            return await _context.AdminLogs
                .Where(l => l.AdminId == adminId)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }
    }
}
