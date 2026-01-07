using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;

namespace DawsonDialGUI.ViewModels.TeacherViewModels;

/// <summary>
/// ViewModel for scheduling events
/// </summary>
public partial class ViewRoomAvailabilityViewModel : ViewModelBase
{
    // Constants
    private const int DEFAULT_START_HOUR = 14;
    private const int DEFAULT_END_HOUR = 15;
    private const int DEFAULT_MINUTES = 0;
    private const int DEFAULT_SECONDS = 0;

    // Observable Properties
    [ObservableProperty] private ObservableCollection<Room> availableRooms = new();
    [ObservableProperty] private DateTimeOffset? date = DateTimeOffset.Now;
    [ObservableProperty] private TimeSpan startTime = new(DEFAULT_START_HOUR, DEFAULT_MINUTES, DEFAULT_SECONDS);
    [ObservableProperty] private TimeSpan endTime = new(DEFAULT_END_HOUR, DEFAULT_MINUTES, DEFAULT_SECONDS);
    [ObservableProperty] private bool showRoomOptions = false;
    [ObservableProperty] private Room selectedRoom = null!;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private string successMessage = string.Empty;

    // Backing fields
    private readonly DawsonDialService _service;
    private readonly Window _mainWindow;
    private readonly Teacher _teacher;

    // Relay commands
    public IRelayCommand LoadAvailableRoomsCommand { get; }
    public IRelayCommand ViewRoomDetailsCommand { get; }
    public IRelayCommand GoBackToSchedulingCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewRoomAvailabilityViewModel"/> class.
    /// </summary>
    /// <param name="teacher">The teacher viewing the event scheduler</param>
    /// <param name="service">The Dawson manager service</param>
    /// <param name="mainWindow">The main avalonia window</param>
    /// <exception cref="ArgumentNullException">If any parameteres are null</exception>
    public ViewRoomAvailabilityViewModel(Teacher teacher, DawsonDialService service, Window mainWindow)
    {
        _teacher = teacher ?? throw new ArgumentNullException(nameof(teacher), "Teacher cannot be null");
        _service = service ?? throw new ArgumentNullException(nameof(service), "Service cannot be null");
        _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow), "Main window cannot be null");

        Initialize(teacher, service, mainWindow);

        LoadAvailableRoomsCommand = new AsyncRelayCommand(LoadAvailableRoomsAsync);
        ViewRoomDetailsCommand = new RelayCommand(ViewRoomDetails);
        GoBackToSchedulingCommand = new RelayCommand(GoBackToScheduling);
    }

    /// <summary>
    /// Loads available rooms for the selected date and time.
    /// </summary>
    /// <returns>Async task</returns>
    private async Task LoadAvailableRoomsAsync()
    {
        ErrorMessage = "";
        AvailableRooms.Clear();

        if (!ValidateDateTime(out DateTime startDateTime, out DateTime endDateTime, out TimeSpan duration))
            return;

        try
        {
            var availableRoomsList = await GetAvailableRoomsAsync(startDateTime, duration);

            if (availableRoomsList.Count == 0)
            {
                ErrorMessage = "No rooms available at the selected time.";
                return;
            }

            PopulateAvailableRooms(availableRoomsList);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error: " + ex.Message;
        }
    }

    /// <summary>
    /// Validates the selected date and time
    /// </summary>
    private bool ValidateDateTime(out DateTime startDateTime, out DateTime endDateTime, out TimeSpan duration)
    {
        startDateTime = DateTime.MinValue;
        endDateTime = DateTime.MinValue;
        duration = TimeSpan.Zero;

        if (Date is null)
        {
            ErrorMessage = "Please select a valid date.";
            return false;
        }

        startDateTime = Date.Value.Date + StartTime;
        endDateTime = Date.Value.Date + EndTime;

        if (startDateTime >= endDateTime)
        {
            ErrorMessage = "End time must be after start time.";
            return false;
        }

        duration = endDateTime - startDateTime;
        return true;
    }

    /// <summary>
    /// Gets available rooms for the selected time
    /// </summary>
    private async Task<List<Room>> GetAvailableRoomsAsync(DateTime startDateTime, TimeSpan duration)
    {
        var allRooms = await _service.GetAllRoomsAsync();
        var availableRooms = new List<Room>();

        foreach (var room in allRooms.Where(r => r is not Office))
        {
            if (await _service.IsRoomAvailableAt(room, NormalizeDateTime(startDateTime), duration))
            {
                availableRooms.Add(room);
            }
        }

        return availableRooms;
    }

    /// <summary>
    /// Populates the available rooms collection
    /// </summary>
    private void PopulateAvailableRooms(List<Room> rooms)
    {
        foreach (var room in rooms)
        {
            AvailableRooms.Add(room);
        }

        ShowRoomOptions = true;
    }

    /// <summary>
    /// Displays the room details for the selected room.
    /// </summary>
    private void ViewRoomDetails()
    {
        if (SelectedRoom is null)
        {
            ErrorMessage = "Please select a room.";
            return;
        }

        var roomDetailsView = new Views.TeacherViews.RoomDetailsView
        {
            DataContext = new RoomDetailsViewModel(SelectedRoom, _teacher, _service, _mainWindow)
        };

        SetCurrentView(roomDetailsView);
    }

    /// <summary>
    /// Navigates back to the Scheduling menu.
    /// </summary>
    private void GoBackToScheduling()
    {
        var view = new Views.TeacherViews.TeacherSchedulingMenuView
        {
            DataContext = new TeacherSchedulingMenuViewModel(_teacher, _service, _mainWindow)
        };
        SetCurrentView(view);
    }

    /// <summary>
    /// Helper method for converting date and time to UTC for postgres
    /// </summary>
    /// <param name="local">The local date and time</param>
    /// <returns>The date and time in utc</returns>
    private DateTime ToUtc(DateTime local) => TimeZoneInfo.ConvertTimeToUtc(local);

    /// <summary>
    /// Normalizes a DateTime to ensure consistent comparison
    /// </summary>
    /// <param name="dateTime">The date time to normalize</param>
    /// <returns>A normalized date time with Kind properly set</returns>
    private DateTime NormalizeDateTime(DateTime dateTime)
    {
        // Ensure the datetime has the correct Kind set (UTC)
        // to avoid timezone comparison issues
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }
}
