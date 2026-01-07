using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DawsonDial.Models.People;
using DawsonDial.Repositories.Interfaces;

namespace DawsonDial.Services.ManagerService
{
    /// <summary>
    /// Provides services for managing administrator-related operations.
    /// </summary>
    public class AdminService : UserService
    {
        private readonly IAdminLogRepository _adminLogRepository;
        private readonly IPersonRepository _personRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminService"/> class.
        /// </summary>
        public AdminService(IPersonRepository personRepository, IAdminLogRepository adminLogRepository)
            : base(personRepository)
        {
            _personRepository = personRepository;
            _adminLogRepository = adminLogRepository;
        }

        /// <summary>
        /// Creates a new user and logs the action by the admin.
        /// </summary>
        public async Task CreateUser(Admin admin, Person newUser)
        {
            await AddUser(newUser);
            admin.CreatedUsersCount++;
            await _personRepository.UpdateAsync(admin);
            await LogAction(admin, $"Created a new user {newUser.Username}");
        }

        /// <summary>
        /// Updates the admin's last login timestamp.
        /// </summary>
        public async Task UpdateLastLogin(Admin admin)
        {
            admin.LastLogin = DateTime.UtcNow;
            await _personRepository.UpdateAsync(admin);
        }

        /// <summary>
        /// Disables a user and logs the action.
        /// </summary>
        public async Task DisableUser(Admin admin, Person user)
        {
            if (user is Admin) throw new InvalidOperationException("You are not authorized to disable another admin!");

            user.IsDisabled = true;
            await _personRepository.UpdateAsync(user);
            await LogAction(admin, $"Disabled user {user.Username}");
        }

        /// <summary>
        /// Enables a user and logs the action.
        /// </summary>
        public async Task EnableUser(Admin admin, Person user)
        {
            user.IsDisabled = false;
            await _personRepository.UpdateAsync(user);
            await LogAction(admin, $"Enabled user {user.Username}");
        }

        public async Task<IEnumerable<AdminLog>> GetAllLogsAsync()
        {
            var allLogs = await _adminLogRepository.GetAllAsync();
            return allLogs;
        }

        /// <summary>
        /// Gets all logs created by a specific admin.
        /// </summary>
        public async Task<IEnumerable<AdminLog>> GetLogsForAdmin(Guid adminId)
        {
            var allLogs = await _adminLogRepository.GetAllAsync();
            return allLogs.Where(log => log.AdminId == adminId);
        }

        /// <summary>
        /// Deletes a user from the system unless the user is an admin.
        /// </summary>
        public async Task RemoveUser(Admin admin, Person user)
        {
            if (user is Admin) throw new InvalidOperationException("You are not authorized to remove another admin!");

            await _personRepository.DeleteAsync(user.PersonId);
            await LogAction(admin, $"Deleted user {user.Username}");
        }

        /// <summary>
        /// Helper method that Logs an action taken by the admin.
        /// </summary>
        public async Task LogAction(Admin admin, string action)
        {
            var log = new AdminLog
            {
                Action = action,
                AdminId = admin.PersonId,
                Timestamp = DateTime.UtcNow
            };

            await _adminLogRepository.AddAsync(log);
        }
    }
}
