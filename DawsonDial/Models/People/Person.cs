using System.Diagnostics.CodeAnalysis;
using DawsonDial.Repositories.Interfaces;
using DawsonDial.Models.Events;
using System.Text.RegularExpressions;

namespace DawsonDial.Models.People
{
    /// <summary>
    /// Represents a person.
    /// </summary>
    public abstract class Person : IAggregateRoot
    {
        // Person properties
        private string _username = string.Empty;
        private string _password = string.Empty;
        private int _age;
        public string UserType => this.GetType().Name;

        public virtual Guid PersonId { get; set; } = Guid.NewGuid();

        public virtual string Username {
            get => _username;
            set {
                if (!Regex.IsMatch(value, @"^[a-zA-Z0-9._%+-]+@dawsoncollege\.qc\.ca$"))
                {
                    throw new ArgumentException("Invalid email format. Must be @dawsoncollege.qc.ca");
                }
                _username = value;
            }
        }
        public virtual string Password {
            get => _password;
            set {
                if (string.IsNullOrEmpty(value) || value.Length < 8)
                {
                    throw new ArgumentException("Password cannot be empty or null and must be at least 8 characters long.");
                }
                _password = value;
            }
        }
        public virtual string FirstName { get; set; } = null!;
        public virtual string LastName { get; set; } = null!;
        public virtual int Age {
            get => _age;
            set {
                if (value < 10 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("Age must be between 10 and 100.");
                }
                _age = value;
            }
         }
        public virtual bool IsDisabled { get; set; }
        public virtual string? Description { get; set; }
        public virtual HashSet<Event> Events { get; set; } = new HashSet<Event>();
        public byte[]? RowVersion { get; set; }

        [SetsRequiredMembers]
        protected Person() { }

        // Person constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Person"/> class.
        /// </summary>
        /// <param name="username">The username of the person.</param>
        /// <param name="password">The password of the person.</param>
        /// <param name="firstName">The first name of the person.</param>
        /// <param name="lastName">The last name of the person.</param>
        /// <param name="age">The age of the person.</param>
        /// <param name="isDisabled">Indicates whether the person is disabled.</param>
        /// <param name="description">The description of the person.</param>
        protected Person(
            string username,
            string password,
            string firstName,
            string lastName,
            int age,
            bool isDisabled,
            string description)
        {
            Username = username;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            IsDisabled = isDisabled;
            Description = description;
        }

        /// <summary>
        /// Adds an event to the person's list of events.
        /// </summary>
        /// <param name="e">The event to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the event is null.</exception>
        public void AddEvent(Event e)
        {
            ArgumentNullException.ThrowIfNull(e);
            if (!Events.Add(e))
            {
                Console.WriteLine("Duplicate event detected, can not add.");
            }
        }

        /// <summary>
        /// Removes an event from the person's list of events.
        /// </summary>
        /// <param name="e">The event to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if the event is null.</exception>
        public void RemoveEvent(Event e)
        {
            ArgumentNullException.ThrowIfNull(e);
            if (!Events.Remove(e))
            {
                Console.WriteLine("Event not found.");
            }
        }

        /// <summary>
        /// Clears all events from the person's list of events.
        /// </summary>
        public void ClearEvents()
        {
            Events.Clear();
        }
    }
}
