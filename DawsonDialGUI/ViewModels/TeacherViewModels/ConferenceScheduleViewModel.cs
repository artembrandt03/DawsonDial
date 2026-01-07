using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.Events;
using DawsonDial.Models.Enums;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;

namespace DawsonDialGUI.ViewModels.TeacherViewModels;

/// <summary>
/// ViewModel for scheduling events
/// </summary>
public partial class ConferenceScheduleViewModel : ViewModelBase
{
    // Constants
    private const int DEFAULT_START_HOUR = 14;
    private const int DEFAULT_END_HOUR = 15;
    private const int DEFAULT_MINUTES = 0;
    private const int DEFAULT_SECONDS = 0;
    private const int NAVIGATION_DELAY_MS = 1500;
    // Observable Properties
    [ObservableProperty] private ObservableCollection<Room> availableRooms = new();
    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private DateTimeOffset? date = DateTimeOffset.Now;
    [ObservableProperty] private TimeSpan startTime = new(DEFAULT_START_HOUR, DEFAULT_MINUTES, DEFAULT_SECONDS);
    [ObservableProperty] private TimeSpan endTime = new(DEFAULT_END_HOUR, DEFAULT_MINUTES, DEFAULT_SECONDS);
    [ObservableProperty] private Room? selectedRoom = null;
    [ObservableProperty] private bool showRoomOptions = false;
    [ObservableProperty] private bool helpMessageVisible = false;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private string successMessage = string.Empty;

    // Backing fields
    private readonly DawsonDialService _service;
    private readonly Window _mainWindow;
    private readonly Teacher _teacher;

    // Relay commands
    public IRelayCommand LoadAvailableRoomsCommand { get; }
    public IRelayCommand EventScheduleCommand { get; }
    public IRelayCommand CancelCommand { get; }
    public IRelayCommand GoBackToSchedulingCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventScheduleViewModel"/> class.
    /// </summary>
    /// <param name="teacher">The teacher viewing the event scheduler</param>
    /// <param name="service">The Dawson manager service</param>
    /// <param name="mainWindow">The main avalonia window</param>
    /// <exception cref="ArgumentNullException">If any parameteres are null</exception>
    public ConferenceScheduleViewModel(Teacher teacher, DawsonDialService service, Window mainWindow)
    {
        _teacher = teacher ?? throw new ArgumentNullException(nameof(teacher), "Teacher cannot be null");
        _service = service ?? throw new ArgumentNullException(nameof(service), "Service cannot be null");
        _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow), "Main window cannot be null");

        Initialize(teacher, service, mainWindow);

        LoadAvailableRoomsCommand = new AsyncRelayCommand(LoadAvailableRoomsAsync);
        EventScheduleCommand = new AsyncRelayCommand(EventScheduleAsync);
        CancelCommand = new RelayCommand(GoBackToMenu);
        GoBackToSchedulingCommand = new RelayCommand(GoBackToScheduling);
    }

    /// <summary>
    /// Loads the available rooms for the selected date and time.
    /// </summary>
    /// <returns>Async task</returns>
    private async Task LoadAvailableRoomsAsync()
    {
        ErrorMessage = string.Empty;
        AvailableRooms.Clear();

        if (!ValidateDateTime())
            return;

        // Calculate date and time values after validation is successful
        DateTime startDateTime = Date!.Value.Date + StartTime;
        TimeSpan duration = (Date.Value.Date + EndTime) - startDateTime;

        try
        {
            await LoadAndFilterAvailableRooms(startDateTime, duration);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error: " + ex.Message;
        }
    }

    /// <summary>
    /// Validates the selected date and time
    /// </summary>
    /// <returns>True if the date and time are valid, false otherwise</returns>
    private bool ValidateDateTime()
    {
        if (Date is null)
        {
            ErrorMessage = "Please select a valid date.";
            return false;
        }

        DateTime startDateTime = Date.Value.Date + StartTime;
        DateTime endDateTime = Date.Value.Date + EndTime;

        if (startDateTime >= endDateTime)
        {
            ErrorMessage = "End time must be after start time.";
            return false;
        }

        return true;
    }

    /// <summary>
    /// Loads and filters available rooms for the selected time
    /// </summary>
    private async Task LoadAndFilterAvailableRooms(DateTime startDateTime, TimeSpan duration)
    {
        var allRooms = await _service.GetAllRoomsAsync();
        var availableRoomsList = new List<Room>();

        foreach (var room in allRooms.Where(r => r is not Office).OrderByDescending(r => r.NumberOfSeats))
        {
            if (await _service.IsRoomAvailableAt(room, NormalizeDateTime(startDateTime), duration))
            {
                availableRoomsList.Add(room);
            }
        }

        if (availableRoomsList.Count == 0)
        {
            ErrorMessage = "No rooms available at the selected time.";
            return;
        }

        foreach (var room in availableRoomsList)
        {
            AvailableRooms.Add(room);
        }

        ShowRoomOptions = true;
        HelpMessageVisible = true;
    }

    /// <summary>
    /// Schedules an event based on the selected date, time, and room.
    /// </summary>
    /// <returns>Async task</returns>
    private async Task EventScheduleAsync()
    {
        if (!ValidateEventInput())
            return;

        try
        {
            if (SelectedRoom is null)
            {
                ErrorMessage = "You must select a room before scheduling.";
                return;
            }

            var newEvent = CreateConferenceEvent();

            if (!await ReserveRoomAndScheduleEvent(newEvent))
                return;

            SuccessMessage = "Event scheduled successfully.";
            await NavigateBackWithSuccessMessage();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error scheduling event: " + ex.Message;
        }
    }

    /// <summary>
    /// Validates the event input parameters
    /// </summary>
    private bool ValidateEventInput()
    {
        ErrorMessage = "";

        if (string.IsNullOrWhiteSpace(Title))
        {
            ErrorMessage = "Event title is required.";
            return false;
        }

        if (Date is null)
        {
            ErrorMessage = "Please select a valid date.";
            return false;
        }

        DateTime startDateTime = Date.Value.Date + StartTime;
        DateTime endDateTime = Date.Value.Date + EndTime;

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
    /// Creates a new conference event with the provided input
    /// </summary>
    private Conference CreateConferenceEvent()
    {
        DateTime startDateTime = Date!.Value.Date + StartTime;
        DateTime endDateTime = Date.Value.Date + EndTime;

        // Normalize dates for event creation
        var normalizedStart = NormalizeDateTime(startDateTime);
        var normalizedEnd = NormalizeDateTime(endDateTime);

        var eventDescription = string.IsNullOrEmpty(Description)
            ? $"Scheduled by {_teacher.FirstName} {_teacher.LastName}"
            : Description;

        var newEvent = new Conference(
            Title,
            eventDescription,
            SelectedRoom!,
            normalizedStart,
            normalizedEnd,
            false,
            EventStatus.Planned,
            _teacher
        );

        newEvent.AddParticipant(_teacher);
        return newEvent;
    }

    /// <summary>
    /// Reserves a room and schedules the event
    /// </summary>
    private async Task<bool> ReserveRoomAndScheduleEvent(Conference newEvent)
    {
        bool reserved = await _service.ReserveRoom(
            SelectedRoom!,
            newEvent,
            newEvent.StartDateTime,
            newEvent.EndDateTime - newEvent.StartDateTime);

        if (!reserved)
        {
            ErrorMessage = "Room reservation failed.";
            return false;
        }

        await _service.ScheduleEvent(newEvent);
        _teacher.AddEvent(newEvent);
        return true;
    }

    /// <summary>
    /// Navigates back with a success message after a short delay
    /// </summary>
    private async Task NavigateBackWithSuccessMessage()
    {
        // Navigate back after short delay to show success message
        await Task.Delay(NAVIGATION_DELAY_MS);
        GoBackToScheduling();
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
        // Ensure datatime has the correct kind set (UTC)
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
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
}
