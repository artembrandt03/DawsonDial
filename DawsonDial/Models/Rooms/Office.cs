using System.Diagnostics.CodeAnalysis;
using DawsonDial.Models.People;

namespace DawsonDial.Models.Rooms
{
    /// <summary>
    /// Represents an office room.
    /// </summary>
    public class Office : Room
    {
        //Attributes
        private readonly Guid _id = Guid.NewGuid();

        public virtual List<Teacher> AssignedTeachers { get; set; } = new List<Teacher>();
        public virtual bool Shared { get; set; } //some offices maybe be split between multiple teachers

        [SetsRequiredMembers]
        protected Office() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Office"/> class.
        /// </summary>
        /// <param name="roomNumber">The room number.</param>
        /// <param name="numberOfSeats">The number of seats.</param>
        /// <param name="isShared">Indicates whether the office is shared.</param>
        public Office(string roomNumber, int numberOfSeats, bool isShared) : base(roomNumber, numberOfSeats)
        {
            Shared = isShared;
            AssignedTeachers = new List<Teacher>();
        }

        //Methods
        /// <summary>
        /// Assigns a teacher to the office.
        /// </summary>
        /// <param name="teacher">The teacher to assign.</param>
        public void AssignTeacher(Teacher teacher)
        {
            AssignedTeachers.Add(teacher);
        }
    }
}
