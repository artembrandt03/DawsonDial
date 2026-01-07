using System.Diagnostics.CodeAnalysis;
using DawsonDial.Models.Events;

namespace DawsonDial.Models.Rooms
{
    /// <summary>
    /// Represents a classroom.
    /// </summary>
    public class ClassRoom : Room
    {
        //Attributes
        private readonly Guid _id = Guid.NewGuid();

        public virtual bool HasProjector { get; set; }
        public virtual bool HasComputers { get; set; } //this if it's a lab for example. should we have another seperate class for labs? or a simple boolean is enough to differentiate i guess?
        public virtual List<ClassSession> AssignedClasses { get; set; } = new List<ClassSession>();

        [SetsRequiredMembers]
        protected ClassRoom() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassRoom"/> class.
        /// </summary>
        /// <param name="roomNumber">The room number.</param>
        /// <param name="numberOfSeats">The number of seats.</param>
        /// <param name="hasProjector">Indicates whether the classroom has a projector.</param>
        /// <param name="hasComputers">Indicates whether the classroom has computers.</param>
        public ClassRoom(string roomNumber, int numberOfSeats, bool hasProjector, bool hasComputers) : base(roomNumber, numberOfSeats)
        {
            HasProjector = hasProjector;
            HasComputers = hasComputers;
            AssignedClasses = new List<ClassSession>();
        }

        //Methods
        /// <summary>
        /// Assigns a class session to the classroom.
        /// </summary>
        /// <param name="classSession">The class session to assign.</param>
        public void AssignClass(ClassSession classSession)
        {
            AssignedClasses.Add(classSession);
        }
    }
}
