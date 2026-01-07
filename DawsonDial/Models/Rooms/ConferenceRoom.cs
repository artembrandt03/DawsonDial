using System.Diagnostics.CodeAnalysis;
using DawsonDial.Models.Events;

namespace DawsonDial.Models.Rooms
{
    /// <summary>
    /// Represents a conference room.
    /// </summary>
    public class ConferenceRoom : Room
    {
        //Attributes
        private readonly Guid _id = Guid.NewGuid();

        public virtual bool HasProjector { get; set; } //would be essential for conference rooms
        public virtual List<Event> ScheduledEvents { get; set; } = new List<Event>();//a list of events for easy tracking by each room

        [SetsRequiredMembers]
        protected ConferenceRoom() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConferenceRoom"/> class.
        /// </summary>
        /// <param name="roomNumber">The room number.</param>
        /// <param name="numberOfSeats">The number of seats.</param>
        /// <param name="hasProjector">Indicates whether the conference room has a projector.</param>
        public ConferenceRoom(string roomNumber, int numberOfSeats, bool hasProjector) : base(roomNumber, numberOfSeats)
        {
            HasProjector = hasProjector;
            ScheduledEvents = new List<Event>();
        }

        //Methods
        /// <summary>
        /// Schedules an event in the conference room.
        /// </summary>
        /// <param name="conference">The conference event to schedule.</param>
        /// <exception cref="InvalidOperationException">Thrown when the conference room is already scheduled during this time.</exception>
        public void ScheduleEvent(Event conference)
        {
            foreach (var existingEvent in ScheduledEvents)
            {
                if (IsTimeOverlapping(existingEvent, conference))
                {
                    throw new InvalidOperationException("The conference room is already scheduled during this time.");
                }
            }
            ScheduledEvents.Add(conference);
        }

        /// <summary>
        /// Checks if the given time overlaps with any existing event.
        /// </summary>
        /// <param name="existing">The existing event to compare against.</param>
        /// <param name="newEvent">The new event to check for overlap.</param>
        /// <returns>True if there is an overlap, otherwise false.</returns>
        private bool IsTimeOverlapping(Event existing, Event newEvent)
        {
            return (newEvent.StartDateTime < existing.EndDateTime &&
                    newEvent.EndDateTime > existing.StartDateTime);
        }
    }
}
