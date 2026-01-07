using System.Diagnostics.CodeAnalysis;
using DawsonDial.Repositories.Interfaces;

namespace DawsonDial.Models.Rooms
{
    /// <summary>
    /// Represents a room.
    /// </summary>
    public class Room: IAggregateRoot
    {
        //Attributes
        public virtual Guid RoomId { get; set; } = Guid.NewGuid();
        public virtual string RoomNumber { get; set; } = null!;
        public virtual int NumberOfSeats { get; set; }
        public virtual bool IsAvailable { get; set; } = true;
        public byte[]? RowVersion { get; set; }

        [SetsRequiredMembers]
        protected Room() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Room"/> class.
        /// </summary>
        /// <param name="roomNumber">The room number.</param>
        /// <param name="numberOfSeats">The number of seats.</param>
        public Room(string roomNumber, int numberOfSeats)
        {
            RoomNumber = roomNumber;
            NumberOfSeats = numberOfSeats;
        }

        /// <summary>
        /// Reserves the room.
        /// </summary>
        public void ReserveRoom() => IsAvailable = false;

        /// <summary>
        /// Releases the room.
        /// </summary>
        public void ReleaseRoom() => IsAvailable = true;

        public override string ToString() => $"{RoomNumber} ({NumberOfSeats} seats)";
    }
}
