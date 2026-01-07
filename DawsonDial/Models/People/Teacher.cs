using System.Diagnostics.CodeAnalysis;
using DawsonDial.Models.Events;
using DawsonDial.Models.Rooms;

namespace DawsonDial.Models.People
{
    /// <summary>
    /// Represents a teacher in the system.
    /// </summary>
    public class Teacher : Person
    {
        public virtual Office? OfficeRoom { get; set; }
        public virtual OfficeHours? OfficeHours { get; set; }
        public virtual string Department { get; set; } = null!;
        public virtual HashSet<Section> Classes { get; set; } = new HashSet<Section>();

        [SetsRequiredMembers]
        protected Teacher() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Teacher"/> class.
        /// </summary>
        /// <param name="username">The username of the teacher.</param>
        /// <param name="password">The password of the teacher.</param>
        /// <param name="firstName">The first name of the teacher.</param>
        /// <param name="lastName">The last name of the teacher.</param>
        /// <param name="age">The age of the teacher.</param>
        /// <param name="isDisabled">Indicates whether the teacher is disabled.</param>
        /// <param name="description">The description of the teacher.</param>
        /// <param name="department">The department of the teacher.</param>
        /// <param name="classes">The classes taught by the teacher.</param>
        /// <param name="officeRoom">The office room of the teacher.</param>
        /// <param name="officeHours">The office hours of the teacher.</param>
        /// <exception cref="ArgumentNullException">Thrown when office room is null.</exception>
        public Teacher(
            string username,
            string password,
            string firstName,
            string lastName,
            int age,
            bool isDisabled,
            string description,
            string department,
            HashSet<Section> classes,
            Office officeRoom,
            OfficeHours officeHours)
            : base(username, password, firstName, lastName, age, isDisabled, description)
        {
            Department = department;
            Classes = classes ?? new HashSet<Section>();
            OfficeRoom = officeRoom;
            OfficeHours = officeHours;
        }

        /// <summary>
        /// Adds a class to the teacher's list of classes.
        /// </summary>
        /// <param name="section">The section to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when section is null.</exception>
        public void AddClass(Section section)
        {
            ArgumentNullException.ThrowIfNull(section);
            if (!Classes.Add(section))
            {
                Console.WriteLine("Duplicate class detected, can not add.");
            }
        }

        /// <summary>
        /// Removes a class from the teacher's list of classes.
        /// </summary>
        /// <param name="section">The section to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown when section is null.</exception>
        public void RemoveClass(Section section)
        {
            ArgumentNullException.ThrowIfNull(section);
            if (!Classes.Remove(section))
            {
                Console.WriteLine("Class not found.");
            }
        }

        /// <summary>
        /// Clears all classes from the teacher's list of classes.
        /// </summary>
        public void ClearClasses()
        {
            Classes.Clear();
        }
    }
}
