using System.Diagnostics.CodeAnalysis;
using DawsonDial.Models.Events;

namespace DawsonDial.Models.People
{
    /// <summary>
    /// Represents an administrator user.
    /// </summary>
    public class Admin : Person
    {
        /// <summary>
        /// Parameterless constructor for EF Core or serialization.
        /// </summary>
        [SetsRequiredMembers]
        protected Admin() { }

        /// <summary>
        /// Constructs a new Admin with provided details.
        /// </summary>
        /// <param name="username">The admin's email address.</param>
        /// <param name="password">The admin's password.</param>
        /// <param name="firstName">The admin's first name.</param>
        /// <param name="lastName">The admin's last name.</param>
        /// <param name="age">The admin's age.</param>
        /// <param name="isDisabled">Indicates if the admin is disabled.</param>
        /// <param name="description">A description of the admin.</param>
        public Admin(
            string username,
            string password,
            string firstName,
            string lastName,
            int age,
            bool isDisabled,
            string description)
            : base(username, password, firstName, lastName, age, isDisabled, description)
        {
        }

        /// <summary>
        /// The last time the admin logged in.
        /// </summary>
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// The number of users this admin has created.
        /// </summary>
        public int CreatedUsersCount { get; set; }

        /// <summary>
        /// A list of logs describing the admin's actions for audit or tracking purposes.
        /// </summary>
        public virtual ICollection<AdminLog> Logs { get; set; } = new List<AdminLog>();

        /// <summary>
        /// Logs an action taken by the admin.
        /// This method adds a new AdminLog entry to the Logs collection.
        /// </summary>
        /// <param name="action">A description of the action performed.</param>
        public void LogAction(string action)
        {
            Logs.Add(new AdminLog
            {
                Action = action,
                Timestamp = DateTime.UtcNow,
                AdminId = this.PersonId,
                Admin = this
            });
        }

        /// <summary>
        /// Increments the counter that tracks how many users this admin has created.
        /// </summary>
        public void IncrementUserCreationCount()
        {
            CreatedUsersCount++;
        }

        /// <summary>
        /// Updates the LastLogin time to the current UTC time.
        /// </summary>
        public void UpdateLastLogin()
        {
            LastLogin = DateTime.UtcNow;
        }
    }
}