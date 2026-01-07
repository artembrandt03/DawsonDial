using System.Diagnostics.CodeAnalysis;
using DawsonDial.Models.Events;

namespace DawsonDial.Models.People
{
    /// <summary>
    /// Represents a student.
    /// </summary>
    public class Student : Person
    {
        // Student-specific properties
        private int _year;
        private int _studentID;
        public virtual int StudentID
        {
            get => _studentID;
            set
            {
                if (value < 1000000 || value > 9999999)
                {
                    throw new ArgumentException("Student ID must be a 7-digit number.");
                }
                _studentID = value;
            }
        }
        public virtual string Program { get; set; } = null!;
        public virtual int Year
        {
            get => _year;
            set
            {
                if (value < 1 || value > 3)
                {
                    throw new ArgumentOutOfRangeException("Year must be between 1 and 3.");
                }
                _year = value;
            }
        }
        public virtual HashSet<Section> Classes { get; set; } = new HashSet<Section>();

        [SetsRequiredMembers]
        protected Student() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Student"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="age">The age.</param>
        /// <param name="isDisabled">Indicates whether the student is disabled.</param>
        /// <param name="description">The student's description.</param>
        /// <param name="studentID">The student's ID.</param>
        /// <param name="program">The student's program.</param>
        /// <param name="year">The student's year.</param>
        /// <param name="classes">The student's classes.</param>
        public Student(
            string username,
            string password,
            string firstName,
            string lastName,
            int age,
            bool isDisabled,
            string description,
            int studentID,
            string program,
            int year,
            HashSet<Section> classes)
            : base(username, password, firstName, lastName, age, isDisabled, description)
        {
            StudentID = studentID;
            Program = program;
            Year = year;
            Classes = classes ?? new HashSet<Section>();
        }

        /// <summary>
        /// Adds a class to the student's list of classes.
        /// </summary>
        /// <param name="section">The class to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the section is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the class is already added.</exception>
        public void AddClass(Section section)
        {
            ArgumentNullException.ThrowIfNull(section);
            if (!Classes.Add(section))
            {
                throw new ArgumentException("Duplicate class detected, can not add.");
            }

            section.EnrolledStudents.Add(this);
        }

        /// <summary>
        /// Removes a class from the student's list of classes.
        /// </summary>
        /// <param name="section">The class to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if the section is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the class is not found.</exception>
        public void RemoveClass(Section section)
        {
            ArgumentNullException.ThrowIfNull(section);
            if (!Classes.Remove(section))
            {
                throw new ArgumentException("Class not found.");
            }
            section.EnrolledStudents.Remove(this);
        }

        /// <summary>
        /// Clears all classes from the student's list of classes.
        /// </summary>
        public void ClearClasses()
        {
            Classes.Clear();
        }
    }
}
