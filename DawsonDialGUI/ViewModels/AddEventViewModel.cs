using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;
using DawsonDial.Services.ManagerService;
using DawsonDialGUI.Views;
using System.Linq;
using DawsonDial.Models.Enums;
using System.Collections.Generic;

namespace DawsonDialGUI.ViewModels;

public partial class AddEventViewModel : ViewModelBase
{
    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private Room? selectedRoom;
    [ObservableProperty] private DateTime startDateTime = DateTime.Now;
    [ObservableProperty] private DateTime endDateTime = DateTime.Now.AddHours(1);
    [ObservableProperty] private bool isRecurring;
    [ObservableProperty] private EventStatus selectedStatus;

    // Dropdown values
    public ObservableCollection<string> EventTypes { get; } = new(Enum.GetNames(typeof(EventType)));
    [ObservableProperty] private string selectedEventType = "Conference";

    public ObservableCollection<Room> Rooms { get; } = new();
    public ObservableCollection<Person> People { get; } = new();
    public ObservableCollection<Teacher> Teachers { get; } = new();
    public ObservableCollection<Course> Courses { get; } = new();

    // Subtype-specific
    [ObservableProperty] private Person? selectedSpeaker;
    [ObservableProperty] private Teacher? selectedTeacher;
    [ObservableProperty] private Course? selectedCourse;
    [ObservableProperty] private string section = string.Empty;
    [ObservableProperty] private string semester = string.Empty;
    [ObservableProperty] private bool isDropIn;

    [ObservableProperty] private string successMessage = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;

    public IRelayCommand SubmitCommand { get; }

    // Date & Time separation
    public DateTimeOffset? StartDate
    {
        get => new DateTimeOffset(StartDateTime.Date);
        set
        {
            if (value.HasValue)
                StartDateTime = new DateTime(value.Value.Year, value.Value.Month, value.Value.Day, StartDateTime.Hour, StartDateTime.Minute, 0);
        }
    }

    public string StartTime
    {
        get => StartDateTime.ToString("HH:mm");
        set
        {
            if (TimeOnly.TryParseExact(value, "HH:mm", out var time))
                StartDateTime = new DateTime(StartDateTime.Year, StartDateTime.Month, StartDateTime.Day, time.Hour, time.Minute, 0);
        }
    }

    public DateTimeOffset? EndDate
    {
        get => new DateTimeOffset(EndDateTime.Date);
        set
        {
            if (value.HasValue)
                EndDateTime = new DateTime(value.Value.Year, value.Value.Month, value.Value.Day, EndDateTime.Hour, EndDateTime.Minute, 0);
        }
    }

    public string EndTime
    {
        get => EndDateTime.ToString("HH:mm");
        set
        {
            if (TimeOnly.TryParseExact(value, "HH:mm", out var time))
                EndDateTime = new DateTime(EndDateTime.Year, EndDateTime.Month, EndDateTime.Day, time.Hour, time.Minute, 0);
        }
    }

    // Dropdown binding for EventStatus
    public IEnumerable<EventStatus> StatusOptions => Enum.GetValues(typeof(EventStatus)).Cast<EventStatus>();

    // Subtype visibility
    public bool IsConference => SelectedEventType == "Conference";
    public bool IsClassSession => SelectedEventType == "ClassSession";
    public bool IsOfficeHours => SelectedEventType == "OfficeHours";
    public bool IsOfficeMeeting => SelectedEventType == "OfficeMeeting";

    public ObservableCollection<Person> Speakers => People;

    public AddEventViewModel(Admin admin, DawsonDialService service, Window mainWindow)
    {
        Initialize(admin, service, mainWindow);
        SubmitCommand = new AsyncRelayCommand(AddEvent);

        _ = LoadDropdowns();
    }

    private async Task LoadDropdowns()
    {
        Rooms.Clear();
        People.Clear();
        Teachers.Clear();
        Courses.Clear();

        foreach (var room in await Service.GetAllRoomsAsync())
            Rooms.Add(room);

        foreach (var person in await Service.GetAllUsersAsync())
            People.Add(person);

        foreach (var teacher in (await Service.GetAllTeachers()))
            Teachers.Add(teacher);

        foreach (var course in await Service.GetAllCoursesAsync())
            Courses.Add(course);
    }

    private async Task AddEvent()
    {
        ErrorMessage = SuccessMessage = string.Empty;

        try
        {
            if (string.IsNullOrWhiteSpace(Title) || SelectedRoom == null)
                throw new Exception("Title and room are required.");

            if (EndDateTime <= StartDateTime)
                throw new Exception("End time must be after start time.");

            StartDateTime = DateTime.SpecifyKind(StartDateTime, DateTimeKind.Utc);
            EndDateTime = DateTime.SpecifyKind(EndDateTime, DateTimeKind.Utc);

            Schedule? generatedSchedule = null;
            if (IsRecurring)
            {
                var slots = new Dictionary<DayOfWeek, (TimeOnly, TimeOnly)>
                {
                    { StartDateTime.DayOfWeek, (TimeOnly.FromDateTime(StartDateTime), TimeOnly.FromDateTime(EndDateTime)) }
                };

                generatedSchedule = new Schedule(
                    slots,
                    DateOnly.FromDateTime(StartDateTime),
                    DateOnly.FromDateTime(EndDateTime));

                await Service.AddSchedule(generatedSchedule);
            }

            Event newEvent = SelectedEventType switch
            {
                "Conference" => new Conference(
                    Title,
                    Description,
                    SelectedRoom,
                    StartDateTime,
                    EndDateTime,
                    IsRecurring,
                    SelectedStatus,
                    SelectedSpeaker ?? throw new Exception("Select a speaker")),

                "ClassSession" => new ClassSession(
                    Title,
                    Description,
                    SelectedRoom,
                    SelectedCourse ?? throw new Exception("Select a course"),
                    Semester,
                    Section,
                    StartDateTime,
                    EndDateTime,
                    IsRecurring,
                    SelectedStatus,
                    generatedSchedule ?? throw new Exception("Schedule required")),

                "OfficeHours" => new OfficeHours(
                    Title,
                    Description,
                    SelectedRoom,
                    StartDateTime,
                    EndDateTime,
                    IsRecurring,
                    SelectedStatus,
                    SelectedTeacher ?? throw new Exception("Select a teacher"),
                    IsDropIn,
                    generatedSchedule ?? throw new Exception("Schedule required for recurring hours")),

                "OfficeMeeting" => new OfficeMeeting(
                    Title,
                    Description,
                    SelectedRoom,
                    StartDateTime,
                    EndDateTime,
                    IsRecurring,
                    SelectedStatus,
                    SelectedTeacher ?? throw new Exception("Select a teacher")),

                _ => throw new Exception("Unsupported event type")
            };

            await Service.ScheduleEvent(newEvent);
            await Service.LogAction((Admin)LoggedInUser, $"Created {SelectedEventType}: \"{Title}\" from {StartDateTime:g} to {EndDateTime:g}");
            SuccessMessage = $"{SelectedEventType} added successfully.";
        }
        catch (Exception ex)
        {
            var fullMessage = ex.Message;
            var inner = ex.InnerException;
            while (inner != null)
            {
                fullMessage += " → " + inner.Message;
                inner = inner.InnerException;
            }

            ErrorMessage = $"Error: {fullMessage}";
        }
    }


    protected override void GoBackToMenu()
    {
        SetCurrentView(new EventMenuView
        {
            DataContext = new EventMenuViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }

    public enum EventType
    {
        Conference,
        ClassSession,
        OfficeHours,
        OfficeMeeting
    }

    partial void OnSelectedEventTypeChanged(string value)
    {
        OnPropertyChanged(nameof(IsConference));
        OnPropertyChanged(nameof(IsClassSession));
        OnPropertyChanged(nameof(IsOfficeHours));
        OnPropertyChanged(nameof(IsOfficeMeeting));
    }
}