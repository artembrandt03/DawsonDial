using System;
using System.Collections.Generic;
using System.Linq;
using DawsonDial.Models.Events;
using DawsonDial.Models.Enums;
using DawsonDial.Models.People;
using DawsonDial.Repositories.Interfaces;

namespace DawsonDial.Services.ManagerService
{
    /// <summary>
    /// Class that provides services to manage events
    /// </summary>
    public class EventService
    {
        //Attributes
        private readonly IEventRepository _eventRepository;

        public IRepository<Event> Events => _eventRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventService"/> class.
        /// </summary>
        /// <param name="events">The event repository.</param>
        /// <exception cref="ArgumentNullException">Thrown when the event repository is null.</exception>
        public EventService(IEventRepository events)
        {
            _eventRepository = events ?? throw new ArgumentNullException(nameof(events));
        }

        /// <summary>
        /// Adds a new event to the internal collection of events.
        /// </summary>
        /// <param name="event">The event to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when the event is null.</exception>
        public async Task AddNewEvent(Event @event)
        {
            ArgumentNullException.ThrowIfNull(@event);
            await _eventRepository.AddAsync(@event);
        }

        /// <summary>
        /// Updates an existing event by replacing it with another one.
        /// </summary>
        /// <param name="updated">The updated event.</param>
        /// <exception cref="ArgumentNullException">Thrown when the original or updated event is null.</exception>
        public async Task UpdateEvent(Event updatedEvent)
        {
            ArgumentNullException.ThrowIfNull(updatedEvent);
            await _eventRepository.UpdateAsync(updatedEvent);
        }

        /// <summary>
        /// Removes an event from the collection.
        /// </summary>
        /// <param name="event">The event to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown when the event is null.</exception>
        public async Task RemoveEvent(Event @event)
        {
            ArgumentNullException.ThrowIfNull(@event);
            Guid eventGuid = @event.EventId;
            await _eventRepository.DeleteAsync(eventGuid);
        }

        /// <summary>
        /// Changes the status of an event.
        /// </summary>
        /// <param name="event">The event to change.</param>
        /// <param name="status">The new status.</param>
        /// <exception cref="ArgumentNullException">Thrown when the event is null.</exception>
        public async Task ChangeEventStatus(Event @event, EventStatus status)
        {
            ArgumentNullException.ThrowIfNull(@event);
            @event.Status = status;
            await _eventRepository.UpdateAsync(@event);
        }

        /// <summary>
        /// Adds a participant to an event.
        /// </summary>
        /// <param name="event">The event to add the participant to.</param>
        /// <param name="participant">The participant to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when the event or participant is null.</exception>
        public async Task AddParticipantToEvent(Event @event, Person participant)
        {
            ArgumentNullException.ThrowIfNull(@event);
            ArgumentNullException.ThrowIfNull(participant);
            @event.AddParticipant(participant);
            await _eventRepository.UpdateAsync(@event);
        }

        /// <summary>
        /// Removes a participant from an event.
        /// </summary>
        /// <param name="event">The event to remove the participant from.</param>
        /// <param name="participant">The participant to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown when the event or participant is null.</exception>
        public async Task RemoveParticipantFromEvent(Event @event, Person participant)
        {
            ArgumentNullException.ThrowIfNull(@event);
            ArgumentNullException.ThrowIfNull(participant);
            @event.RemoveParticipant(participant);
            await _eventRepository.UpdateAsync(@event);
        }

        /// <summary>
        /// Checks if an event has a participant.
        /// </summary>
        /// <param name="event">The event to check.</param>
        /// <param name="participant">The participant to check for.</param>
        /// <returns>True if the event has the participant, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the event or participant is null.</exception>
        public async Task<bool> CheckEventHasParticipant(Event @event, Person person)
        {
            ArgumentNullException.ThrowIfNull(@event);
            ArgumentNullException.ThrowIfNull(person);

            var eventEntity = await _eventRepository.GetByIdAsync(@event.EventId);
            return eventEntity.Participants.Contains(person);
        }

        /// <summary>
        /// Checks if two events have overlapping times (schedule conflict).
        /// </summary>
        /// <param name="event1">The first event to check.</param>
        /// <param name="event2">The second event to check.</param>
        /// <returns>True if the events have overlapping times, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either event is null.</exception>
        public bool CheckScheduleConflicts(Event event1, Event event2)
        {
            ArgumentNullException.ThrowIfNull(event1);
            ArgumentNullException.ThrowIfNull(event2);

            return event1.StartDateTime < event2.EndDateTime &&
                   event1.EndDateTime > event2.StartDateTime;
        }

        /// <summary>
        /// Retrieves an event by its ID.
        /// </summary>
        /// <param name="id">The ID of the event to retrieve.</param>
        /// <returns>The event with the given ID.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the event with the given ID was not found.</exception>
        public async Task<Event> GetEventById(Guid id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            return eventEntity;
        }

        /// <summary>
        /// Retrieves all events.
        /// </summary>
        /// <returns>A set of all events.</returns>
        public async Task<IEnumerable<Event>> GetAllEvents()
        {
            var eventList = await _eventRepository.GetAllAsync();
            return eventList;
        }
    }
}
