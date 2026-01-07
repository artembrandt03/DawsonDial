using System.Diagnostics.CodeAnalysis;
using DawsonDial.Models.Enums;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;

namespace DawsonDial.Models.Events
{
    /// <summary>
    /// Represents a conference event.
    /// </summary>
    public class Conference : Event
    {
        // Conference specific properties
        public virtual Person? Speaker { get; set; }

        [SetsRequiredMembers]
        protected Conference() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Conference"/> class.
        /// </summary>
        /// <param name="title">The title of the conference.</param>
        /// <param name="description">The description of the conference.</param>
        /// <param name="room">The room where the conference will be held.</param>
        /// <param name="startDateTime">The start date and time of the conference.</param>
        /// <param name="endDateTime">The end date and time of the conference.</param>
        /// <param name="isRecurring">Indicates whether the conference is recurring.</param>
        /// <param name="status">The status of the conference.</param>
        /// <param name="speaker">The speaker of the conference.</param>
        /// <exception cref="ArgumentNullException">Thrown if speaker is null.</exception>
        public Conference(
        string title,
        string description,
        Room room,
        DateTime startDateTime,
        DateTime endDateTime,
        bool isRecurring,
        EventStatus status,
        Person speaker)
        : base(title, description, room, startDateTime, endDateTime, isRecurring, status)
        {
            Speaker = speaker ?? throw new ArgumentNullException(nameof(speaker));
        }

        // Conference Methods
        /// <summary>
        /// Registers an attendee for the conference.
        /// </summary>
        /// <param name="attendee">The attendee to register.</param>
        public void RegisterAttendee(Person attendee) => AddParticipant(attendee);

        /// <summary>
        /// Unregisters an attendee from the conference.
        /// </summary>
        /// <param name="attendee">The attendee to unregister.</param>
        public void UnregisterAttendee(Person attendee) => RemoveParticipant(attendee);

        /// <summary>
        /// Checks if an attendee is registered for the conference.
        /// </summary>
        /// <param name="attendee">The attendee to check.</param>
        /// <returns>True if the attendee is registered, false otherwise.</returns>
        public bool IsAttendeeRegistered(Person attendee) => HasParticipant(attendee);
    }
}
