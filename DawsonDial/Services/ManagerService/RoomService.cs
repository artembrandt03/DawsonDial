using DawsonDial.Models.Rooms;
using DawsonDial.Models.Events;
using DawsonDial.Repositories.Interfaces;
using DawsonDial.Models.Enums;

namespace DawsonDial.Services.ManagerService
{
    /// <summary>
    /// Provides asynchronous room management services with integrated database access.
    /// Supports CRUD operations and reservation logic using a repository pattern.
    /// </summary>
    public class RoomService
    {
        // RoomService Backing Fields
        private readonly IRoomRepository _roomRepository;
        private readonly IEventRepository _eventRepository;

        /// <summary>
        /// Initializes a new instance of the RoomService class with the provided repositories.
        /// </summary>
        public RoomService(IRoomRepository roomRepository, IEventRepository? eventRepository = null)
        {
            _roomRepository = roomRepository;
            _eventRepository = eventRepository!;
        }

        /// <summary>
        /// Asynchronously adds a room to the database.
        /// </summary>
        public async Task AddRoomAsync(Room roomToAdd)
        {
            ArgumentNullException.ThrowIfNull(roomToAdd);
            await _roomRepository.AddAsync(roomToAdd);
        }

        /// <summary>
        /// Asynchronously updates a room in the database.
        /// </summary> 
        public async Task UpdateRoomAsync(Room roomToUpdate)
        {
            ArgumentNullException.ThrowIfNull(roomToUpdate);
            await _roomRepository.UpdateAsync(roomToUpdate);
        }

        /// <summary>
        /// Asynchronously removes a room from the database using its ID.
        /// </summary>
        public async Task RemoveRoomAsync(Guid roomId)
        {
            await _roomRepository.DeleteAsync(roomId);
        }

        /// <summary>
        /// Asynchronously retrieves all rooms from the database.
        /// </summary>
        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _roomRepository.GetAllAsync();
        }

        /// <summary>
        /// Attempts to reserve a room for a specific time range.
        /// </summary>
        public async Task<bool> ReserveRoom(Room room, Event eventToSchedule, DateTime dateTime, TimeSpan timeSpan)
        {
            ArgumentNullException.ThrowIfNull(room);
            ArgumentNullException.ThrowIfNull(eventToSchedule);

            if (timeSpan <= TimeSpan.Zero)
                throw new ArgumentException("Time span cannot be zero or negative.");
            if (dateTime < DateTime.Now)
                throw new ArgumentException("Reservation date cannot be in the past.");
            if (room.NumberOfSeats < eventToSchedule.Participants.Count)
                throw new ArgumentException("Room does not have enough seats for the event.");

            // Normalize input times
            var normalizedStart = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            var endTime = dateTime + timeSpan;
            var normalizedEnd = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);

            // Check if room is available for the requested time
            if (!await IsRoomAvailableAt(room, normalizedStart, timeSpan))
                return false;

            // Update event Room if it's not already set
            if (eventToSchedule.Room != room)
            {
                eventToSchedule.Room = room;
            }

            // Set the normalized start and end times
            eventToSchedule.StartDateTime = normalizedStart;
            eventToSchedule.EndDateTime = normalizedEnd;

            return true;
        }

        /// <summary>
        /// Releases a reservation for a room at a given time, if it exists.
        /// </summary>
        public async Task<bool> ReleaseRoom(Room room, DateTime dateTime)
        {
            ArgumentNullException.ThrowIfNull(room);

            if (dateTime < DateTime.Now)
                throw new ArgumentException("Cannot release room in the past.");

            if (_eventRepository == null)
                return false;

            // Normalize input time
            var normalizedDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

            // Find events scheduled for this room at the specified time
            var events = await _eventRepository.GetAllAsync();

            // Get the first matching event
            var eventToCancel = events.FirstOrDefault(e =>
                e.Room?.RoomId == room.RoomId &&
                e.Status != EventStatus.Cancelled &&
                normalizedDateTime >= e.StartDateTime &&
                normalizedDateTime <= e.EndDateTime);

            if (eventToCancel == null)
                return false;

            // Update the event to cancelled status
            eventToCancel.Status = EventStatus.Cancelled;
            try
            {
                await _eventRepository.UpdateAsync(eventToCancel);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating event in database: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Asynchronously retrieves a room by its unique identifier.
        /// </summary>
        public async Task<Room> GetRoomByIdAsync(Guid roomId)
        {
            return await _roomRepository.GetByIdAsync(roomId);
        }

        /// <summary>
        /// Determines whether the specified room is available at the current moment.
        /// </summary>
        public async Task<bool> IsRoomCurrentlyAvailable(Room room)
        {
            ArgumentNullException.ThrowIfNull(room);

            if (!room.IsAvailable)
                return false;

            if (_eventRepository == null)
                return true;

            // Normalize current time
            DateTime normalizedNow = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            // Get all events
            var events = await _eventRepository.GetAllAsync();

            // Check if any event is using this room right now
            bool isBooked = events.Any(e =>
                e.Room?.RoomId == room.RoomId &&
                e.Status != EventStatus.Cancelled &&
                normalizedNow >= e.StartDateTime &&
                normalizedNow < e.EndDateTime);

            return !isBooked;
        }

        /// <summary>
        /// Checks whether a room is free for a specific time slot.
        /// </summary>
        public async Task<bool> IsRoomAvailableAt(Room room, DateTime dateTime, TimeSpan duration)
        {
            ArgumentNullException.ThrowIfNull(room);

            if (dateTime < DateTime.Now)
                throw new ArgumentException("Date time is in the past.");
            if (duration <= TimeSpan.Zero)
                throw new ArgumentException("Duration cannot be zero or negative.");

            if (_eventRepository == null)
                return true;

            DateTime endTime = dateTime.Add(duration);

            // Normalize the input times for comparison
            var normalizedStart = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            var normalizedEnd = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);

            // Get all events
            var events = await _eventRepository.GetAllAsync();

            // Check if any event overlaps with the requested time
            bool isOverlapping = events.Any(e =>
                e.Room?.RoomId == room.RoomId &&
                e.Status != EventStatus.Cancelled &&
                normalizedStart < e.EndDateTime &&
                normalizedEnd > e.StartDateTime);

            return !isOverlapping;
        }

        /// <summary>
        /// Asynchronously returns all rooms that are available at a specific date and time.
        /// </summary>
        public async Task<HashSet<Room>> AvailableRoomsAtAsync(DateTime dateTime)
        {
            if (dateTime < DateTime.Now)
                throw new ArgumentException("Cannot check availability for a time in the past.");

            var allRooms = await _roomRepository.GetAllAsync();
            var availableRooms = new HashSet<Room>();

            // Normalize input time
            var normalizedDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

            // Get all events
            var events = await _eventRepository.GetAllAsync();

            // Filter to just active events at the specified time
            var activeEvents = events.Where(e =>
                e.Status != EventStatus.Cancelled &&
                normalizedDateTime >= e.StartDateTime &&
                normalizedDateTime < e.EndDateTime).ToList();

            // Get the room IDs that are booked
            var bookedRoomIds = activeEvents.Where(e => e.Room != null)
                                            .Select(e => e.Room!.RoomId)
                                            .ToHashSet();

            // Add all rooms that aren't in the booked list
            foreach (var room in allRooms)
            {
                if (!bookedRoomIds.Contains(room.RoomId))
                {
                    availableRooms.Add(room);
                }
            }

            return availableRooms;
        }

        /// <summary>
        /// Marks all past events as Completed if they haven't been explicitly cancelled.
        /// </summary>
        public async Task MarkPastEventsAsCompleted()
        {
            if (_eventRepository == null)
                return;
            // Get current time in UTC
            var currentTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            // Get all events
            var events = await _eventRepository.GetAllAsync();

            // Find events that have ended and are not cancelled or already completed
            var eventsToUpdate = events.Where(e =>
                e.EndDateTime < currentTime &&
                e.Status != EventStatus.Cancelled &&
                e.Status != EventStatus.Completed).ToList();

            // Update events to completed status
            foreach (var evt in eventsToUpdate)
            {
                evt.Status = EventStatus.Completed;
                try
                {
                    await _eventRepository.UpdateAsync(evt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating event to Completed: {ex.Message}");
                }
            }
        }
    }
}
