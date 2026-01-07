using System.Diagnostics.CodeAnalysis;
using DawsonDial.Repositories.Interfaces;
using DawsonDial.Models.Enums;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;

namespace DawsonDial.Models.Events
{
    /// <summary>
    /// Represents an event.
    /// </summary>
    public abstract class Event : IAggregateRoot
    {
        // Common Event Properties
        public virtual Guid EventId { get; set; } = Guid.NewGuid();
        public virtual string Title { get; set; } = null!;
        public virtual string? Description { get; set; }
        public virtual Room Room { get; set; } = null!;
        public virtual int MaxCapacity => Room.NumberOfSeats;
        public virtual int AvailableSeats => MaxCapacity - Participants.Count;
        public virtual bool IsFull => Participants.Count >= MaxCapacity;
        public virtual DateTime StartDateTime { get; set; }
        public virtual DateTime EndDateTime { get; set; }
        public virtual bool IsRecurring { get; set; }
        public virtual EventStatus Status { get; set; }
        public virtual HashSet<Person> Participants { get; set; } = new HashSet<Person>();
        public byte[]? RowVersion { get; set; }

        [SetsRequiredMembers]
        protected Event() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="title">The title of the event.</param>
        /// <param name="description">The description of the event.</param>
        /// <param name="room">The room where the event will take place.</param>
        /// <param name="startDateTime">The start date and time of the event.</param>
        /// <param name="endDateTime">The end date and time of the event.</param>
        /// <param name="isRecurring">Indicates whether the event is recurring.</param>
        /// <param name="status">The status of the event.</param>
        /// <exception cref="ArgumentException">Thrown when the title or description is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when the start date time is after the end date time.</exception>
        protected Event(
            string title,
            string description,
            Room room,
            DateTime startDateTime,
            DateTime endDateTime,
            bool isRecurring,
            EventStatus status)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be null or empty.", nameof(description));
            if (startDateTime > endDateTime)
                throw new ArgumentException("Start date cannot be after end date.", nameof(startDateTime));

            Title = title;
            Description = description;
            Room = room ?? throw new ArgumentNullException(nameof(room));
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            IsRecurring = isRecurring;
            Status = status;
        }

        // Event Methods
        /// <summary>
        /// Adds a participant to the event.
        /// </summary>
        /// <param name="participant">The participant to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when the participant is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the event is full or the participant is already registered.</exception>
        public virtual void AddParticipant(Person participant)
        {
            ArgumentNullException.ThrowIfNull(participant);
            if (IsFull)
            {
                throw new InvalidOperationException("Event is full.");
            }
            if (HasParticipant(participant))
            {
                throw new InvalidOperationException("Participant is already registered for this event.");
            }
            Participants.Add(participant);
        }

        /// <summary>
        /// Removes a participant from the event.
        /// </summary>
        /// <param name="participant">The participant to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown when the participant is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the participant is not registered for this event.</exception>
        public virtual void RemoveParticipant(Person participant)
        {
            ArgumentNullException.ThrowIfNull(participant);
            Participants.Remove(participant);
        }

        /// <summary>
        /// Checks if a participant is registered for the event.
        /// </summary>
        /// <param name="participant">The participant to check.</param>
        /// <exception cref="ArgumentNullException">Thrown when the participant is null.</exception>
        /// <returns>True if the participant is registered for the event, false otherwise.</returns>
        public bool HasParticipant(Person participant)
        {
            ArgumentNullException.ThrowIfNull(participant);
            return Participants.Contains(participant);
        }
    }
}
