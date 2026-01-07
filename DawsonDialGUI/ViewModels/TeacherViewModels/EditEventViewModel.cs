using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.Enums;
using DawsonDial.Services.ManagerService;

namespace DawsonDialGUI.ViewModels.TeacherViewModels
{
    /// <summary>
    /// ViewModel for editing an event.
    /// </summary>
    public partial class EditEventViewModel : ViewModelBase
    {
        // Constants
        private const int DEFAULT_START_HOUR = 14;
        private const int DEFAULT_END_HOUR = 15;
        private const int DEFAULT_MINUTES = 0;
        private const int DEFAULT_SECONDS = 0;
        private const int NAVIGATION_DELAY_MS = 1500;

        private readonly Event _event;
        private readonly DawsonDialService _service;
        private readonly Teacher _teacher;
        private readonly Window _mainWindow;

        [ObservableProperty] private string title = string.Empty;
        [ObservableProperty] private string description = string.Empty;
        [ObservableProperty] private DateTimeOffset date = DateTimeOffset.Now;
        [ObservableProperty] private TimeSpan startTime = new(DEFAULT_START_HOUR, DEFAULT_MINUTES, DEFAULT_SECONDS);
        [ObservableProperty] private TimeSpan endTime = new(DEFAULT_END_HOUR, DEFAULT_MINUTES, DEFAULT_SECONDS);

        private Room _selectedRoom;
        [ObservableProperty] private RoomViewModel _selectedRoomViewModel;
        [ObservableProperty] private ObservableCollection<RoomViewModel> availableRooms = new();
        [ObservableProperty] private string errorMessage = string.Empty;
        [ObservableProperty] private string successMessage = string.Empty;
        [ObservableProperty] private string helpMessage = string.Empty;

        // Property to bind to the Room directly
        public Room SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                if (_selectedRoom != value)
                {
                    _selectedRoom = value;

                    // Find or create a matching view model
                    if (value != null)
                    {
                        var viewModel = AvailableRooms.FirstOrDefault(vm => vm.Room?.RoomId == value.RoomId);
                        if (viewModel == null)
                        {
                            // Create a new view model if not found
                            viewModel = new RoomViewModel(value);
                        }
                        SelectedRoomViewModel = viewModel;
                    }
                    else
                    {
                        SelectedRoomViewModel = null;
                    }
                }
            }
        }

        // When the view model changes, update the actual Room
        partial void OnSelectedRoomViewModelChanged(RoomViewModel value)
        {
            _selectedRoom = value?.Room;
        }

        public IRelayCommand SaveCommand { get; }
        public IRelayCommand BackToEventsCommand { get; }
        public IRelayCommand CheckAvailableRoomsCommand { get; }

        /// <summary>
        /// Initializes a new instance of the EditEventViewModel class.
        /// </summary>
        /// <param name="evt">The event to edit.</param>
        /// <param name="teacher">The teacher editing the event.</param>
        /// <param name="service">The service to use for updating the event.</param>
        /// <param name="mainWindow">The main window.</param>
        public EditEventViewModel(Event evt, Teacher teacher, DawsonDialService service, Window mainWindow)
        {
            _event = evt ?? throw new ArgumentNullException(nameof(evt), "Event cannot be null");
            _teacher = teacher ?? throw new ArgumentNullException(nameof(teacher), "Teacher cannot be null");
            _service = service ?? throw new ArgumentNullException(nameof(service), "Service cannot be null");
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow), "Main window cannot be null");

            Initialize(teacher, service, mainWindow);

            // Initialize properties with event values
            Title = evt.Title;
            Description = evt.Description!;
            Date = evt.StartDateTime;
            StartTime = evt.StartDateTime.TimeOfDay;
            EndTime = evt.EndDateTime.TimeOfDay;
            _selectedRoom = evt.Room;

            SaveCommand = new AsyncRelayCommand(SaveChangesAsync);
            BackToEventsCommand = new RelayCommand(BackToMyEvents);
            CheckAvailableRoomsCommand = new AsyncRelayCommand(CheckAvailableRoomsAsync);

            // Load all rooms initially
            LoadAllRoomsAsync();
        }

        /// <summary>
        /// Loads all rooms
        /// </summary>
        private async void LoadAllRoomsAsync()
        {
            try
            {
                ErrorMessage = string.Empty;

                var rooms = await _service.GetAllRoomsAsync();
                var filteredRooms = CreateSortedRoomViewModels(rooms);
                AvailableRooms = new ObservableCollection<RoomViewModel>(filteredRooms);

                if (_selectedRoom != null)
                {
                    SelectedRoom = _selectedRoom;
                    SetInitialSelectedRoom();
                }

                HelpMessage = "Click 'Check Available Rooms' to see rooms available at the selected time.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading rooms: {ex.Message}";
            }
        }

        /// <summary>
        /// Sets the initial selected room in the UI
        /// </summary>
        private void SetInitialSelectedRoom()
        {
            if (SelectedRoom != null)
            {
                var viewModel = AvailableRooms.FirstOrDefault(vm => vm.Room?.RoomId == SelectedRoom.RoomId);
                if (viewModel != null)
                {
                    SelectedRoomViewModel = viewModel;
                }
            }
        }

        /// <summary>
        /// Check for available rooms at the selected date and time
        /// </summary>
        private async Task CheckAvailableRoomsAsync()
        {
            ResetMessages();

            try
            {
                // Create date/time objects
                DateTime startDateTime = Date.Date + StartTime;
                DateTime endDateTime = Date.Date + EndTime;

                // Validate date/time
                if (!ValidateDateTime(startDateTime, endDateTime))
                    return;

                var availableRoomsList = await GetAvailableRoomsAsync(startDateTime, endDateTime);
                UpdateAvailableRoomsDisplay(availableRoomsList);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error checking room availability: {ex.Message}";
            }
        }

        /// <summary>
        /// Gets a list of available rooms for the selected time
        /// </summary>
        private async Task<List<Room>> GetAvailableRoomsAsync(DateTime startDateTime, DateTime endDateTime)
        {
            var allRooms = await _service.GetAllRoomsAsync();
            var availableRoomsList = new List<Room>();

            foreach (var room in allRooms)
            {
                if (room.RoomId == _event.Room.RoomId)
                {
                    availableRoomsList.Add(room);
                    continue;
                }

                bool isAvailable = await _service.IsRoomAvailableAt(
                    room,
                    NormalizeDateTime(startDateTime),
                    endDateTime - startDateTime);

                if (isAvailable)
                {
                    availableRoomsList.Add(room);
                }
            }

            return availableRoomsList;
        }

        /// <summary>
        /// Updates the UI with available rooms
        /// </summary>
        private void UpdateAvailableRoomsDisplay(List<Room> availableRoomsList)
        {
            if (availableRoomsList.Count == 0)
            {
                ErrorMessage = "No rooms available at the selected time.";
                return;
            }

            var availableRoomViewModels = CreateSortedRoomViewModels(availableRoomsList);
            AvailableRooms = new ObservableCollection<RoomViewModel>(availableRoomViewModels);
            UpdateRoomSelection();

            SuccessMessage = $"Found {availableRoomsList.Count} available room(s).";
        }

        /// <summary>
        /// Updates the room selection to maintain the current selection if possible
        /// </summary>
        private void UpdateRoomSelection()
        {
            if (SelectedRoom == null)
                return;

            var viewModel = AvailableRooms.FirstOrDefault(vm => vm.Room?.RoomId == SelectedRoom.RoomId);
            if (viewModel != null)
            {
                SelectedRoomViewModel = viewModel;
            }
            else if (AvailableRooms.Count > 0)
            {
                SelectedRoomViewModel = AvailableRooms.First();
            }
        }

        /// <summary>
        /// Creates sorted room view models from a list of rooms
        /// </summary>
        private List<RoomViewModel> CreateSortedRoomViewModels(IEnumerable<Room> rooms)
        {
            return rooms
                .Where(r => r != null)
                .Select(r => new RoomViewModel(r))
                .OrderBy(r => GetRoomTypeSortOrder(r.Room))
                .ThenBy(r => r.RoomNumber)
                .ToList();
        }

        /// <summary>
        /// Resets all message fields
        /// </summary>
        private void ResetMessages()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            HelpMessage = string.Empty;
        }

        /// <summary>
        /// Validates the date and time
        /// </summary>
        private bool ValidateDateTime(DateTime startDateTime, DateTime endDateTime)
        {
            if (startDateTime >= endDateTime)
            {
                ErrorMessage = "End time must be after start time.";
                return false;
            }

            if (startDateTime < DateTime.Now)
            {
                ErrorMessage = "Cannot schedule an event in the past.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Saves the event changes.
        /// </summary>
        private async Task SaveChangesAsync()
        {
            ResetMessages();

            try
            {
                // Create date/time objects
                DateTime startDateTime = Date.Date + StartTime;
                DateTime endDateTime = Date.Date + EndTime;

                if (!ValidateEventInput(startDateTime, endDateTime))
                    return;

                if (!await CheckRoomAvailabilityAsync(startDateTime, endDateTime))
                    return;

                await UpdateEventAsync(startDateTime, endDateTime);
                await NavigateBackWithSuccessMessage();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error saving event: {ex.Message}";
            }
        }

        /// <summary>
        /// Validates event input fields
        /// </summary>
        private bool ValidateEventInput(DateTime startDateTime, DateTime endDateTime)
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Event title is required.";
                return false;
            }

            if (!ValidateDateTime(startDateTime, endDateTime))
                return false;

            if (SelectedRoom == null)
            {
                ErrorMessage = "A room must be selected.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the selected room is available at the specified time
        /// With custom logic for same room but time changed (to extend/reduce duration, etc)
        /// </summary>
        private async Task<bool> CheckRoomAvailabilityAsync(DateTime startDateTime, DateTime endDateTime)
        {
            // Check room availability if it changed
            if (SelectedRoom.RoomId != _event.Room.RoomId)
            {
                bool isAvailable = await _service.IsRoomAvailableAt(
                    SelectedRoom,
                    NormalizeDateTime(startDateTime),
                    endDateTime - startDateTime);

                if (!isAvailable)
                {
                    ErrorMessage = "Selected room is not available at the specified time.";
                    return false;
                }
            }
            // If same room but time changed, we need custom availability checking
            else if (_event.StartDateTime != startDateTime || _event.EndDateTime != endDateTime)
            {
                var allEvents = await _service.GetAllEventsAsync();

                // Look for conflicts excluding the current event being edited
                bool hasConflict = allEvents
                    .Where(e => e.EventId != _event.EventId)
                    .Where(e => e.Room?.RoomId == SelectedRoom.RoomId)
                    .Where(e => e.Status != EventStatus.Cancelled)
                    .Any(e =>
                        NormalizeDateTime(startDateTime) < e.EndDateTime &&
                        NormalizeDateTime(endDateTime) > e.StartDateTime);

                if (hasConflict)
                {
                    ErrorMessage = "Room is not available at the new time.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Updates the event with new values
        /// </summary>
        private async Task UpdateEventAsync(DateTime startDateTime, DateTime endDateTime)
        {
            // Update event properties
            _event.Title = Title;
            _event.Description = Description;
            _event.StartDateTime = NormalizeDateTime(startDateTime);
            _event.EndDateTime = NormalizeDateTime(endDateTime);

            if (_event.Room.RoomId != SelectedRoom.RoomId)
            {
                _event.Room = SelectedRoom;
            }

            await _service.UpdateEventAsync(_event);
            SuccessMessage = "Event updated successfully.";
        }

        /// <summary>
        /// Navigates back with a success message after a short delay
        /// </summary>
        private async Task NavigateBackWithSuccessMessage()
        {
            // Navigate back after short delay to show success message
            await Task.Delay(NAVIGATION_DELAY_MS);
            BackToMyEvents();
        }

        /// <summary>
        /// Normalizes a DateTime to ensure consistent comparison
        /// </summary>
        /// <param name="dateTime">The date time to normalize</param>
        /// <returns>A normalized date time with Kind properly set</returns>
        private DateTime NormalizeDateTime(DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        /// <summary>
        /// Navigates back to the View My Events screen.
        /// </summary>
        private async void BackToMyEvents()
        {
            var vm = await ViewMyEventsViewModel.CreateAsync(_teacher, _service, _mainWindow);
            var view = new Views.TeacherViews.ViewMyEventsView
            {
                DataContext = vm
            };
            SetCurrentView(view);
        }

        /// <summary>
        /// Gets a sort order value for room types
        /// </summary>
        /// <param name="room">The room to get a sort order for</param>
        /// <returns>Integer representing sort order (lower numbers first)</returns>
        private static int GetRoomTypeSortOrder(Room room)
        {
            return room switch
            {
                ConferenceRoom => 1,
                ClassRoom => 2,
                Office => 3,
                _ => 99
            };
        }
    }

    /// <summary>
    /// A helper class for displaying room information in the UI
    /// </summary>
    public class RoomViewModel
    {
        public Room Room { get; }
        public string RoomNumber => Room?.RoomNumber ?? "N/A";
        public string Type => Room?.GetType().Name ?? "Unknown";
        public int NumberOfSeats => Room?.NumberOfSeats ?? 0;
        public string DisplayText { get; }

        /// <summary>
        /// Initializes a new instance of the RoomViewModel class.
        /// </summary>
        /// <param name="room">The room to display</param>
        public RoomViewModel(Room room)
        {
            Room = room;
            DisplayText = GenerateDisplayText(room);
        }

        /// <summary>
        /// Generates a display text for the room
        /// </summary>
        /// <param name="room">The room to generate a display text for</param>
        /// <returns>A string representing the room's details</returns>
        private string GenerateDisplayText(Room room)
        {
            if (room == null)
            {
                return "No room selected";
            }

            var details = $"{room.RoomNumber} - {room.GetType().Name} ({room.NumberOfSeats} seats)";

            if (room is ClassRoom classRoom)
            {
                details += $"\nProjector: {classRoom.HasProjector}, Computers: {classRoom.HasComputers}";
            }
            else if (room is ConferenceRoom conferenceRoom)
            {
                details += $"\nProjector: {conferenceRoom.HasProjector}";
            }
            else if (room is Office office)
            {
                details += $"\nShared Office: {office.Shared}";
            }

            return details;
        }

        // Implicit operator to convert between RoomViewModel and Room
        public static implicit operator Room(RoomViewModel viewModel) => viewModel?.Room;
    }
}