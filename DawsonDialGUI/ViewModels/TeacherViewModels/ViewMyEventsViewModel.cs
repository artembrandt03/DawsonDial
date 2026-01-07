using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.Enums;
using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels.TeacherViewModels;

/// <summary>
/// ViewModel for displaying a teacher's upcoming events.
/// </summary>
public partial class ViewMyEventsViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<EventViewModel> upcomingEvents = new();

    [ObservableProperty] private string pageTitle = string.Empty;

    public IRelayCommand GoBackToEventMenuCommand { get; }
    public IRelayCommand RefreshEventsCommand { get; }

    /// <summary>
    /// Initializes the ViewModel.
    /// </summary>
    private ViewMyEventsViewModel()
    {
        GoBackToEventMenuCommand = new RelayCommand(GoBackToEventMenu);
        RefreshEventsCommand = new RelayCommand(RefreshEvents);
    }

    /// <summary>
    /// Refreshes the events.
    /// </summary>
    private async void RefreshEvents()
    {
        await LoadTeacherEventsAsync((Teacher)LoggedInUser);
    }

    /// <summary>
    /// Creates and initializes the ViewModel asynchronously.
    /// </summary>
    /// <param name="teacher">The teacher to display the events for.</param>
    /// <param name="service">The service to use for the events.</param>
    /// <param name="mainWindow">The main window to use for the events.</param>
    /// <returns>The ViewModel.</returns>
    public static async Task<ViewMyEventsViewModel> CreateAsync(Teacher teacher, DawsonDialService service, Window mainWindow)
    {
        var vm = new ViewMyEventsViewModel();
        vm.Initialize(teacher, service, mainWindow);
        await vm.LoadTeacherEventsAsync(teacher);
        return vm;
    }

    /// <summary>
    /// Loads the teacher's events from the database, sets past events as completed, and filters to show only upcoming ones.
    /// </summary>
    /// <param name="teacher">The teacher to display the events for.</param>
    /// <returns>The async task.</returns>
    private async Task LoadTeacherEventsAsync(Teacher teacher)
    {
        // Mark past events as completed
        await Service.MarkPastEventsAsCompleted();

        // Filter and transform events
        var events = FilterAndTransformEvents(teacher);

        UpcomingEvents = new ObservableCollection<EventViewModel>(events);
        PageTitle = $"{teacher.FirstName}'s Upcoming Events";
    }

    /// <summary>
    /// Filters the teacher's events to show only upcoming ones and transforms them into view models
    /// </summary>
    /// <param name="teacher">The teacher viewing the events</param>
    /// <returns>Filtered and ordered list of event view models</returns>
    private List<EventViewModel> FilterAndTransformEvents(Teacher teacher)
    {
        return teacher.Events
            .Where(e => e.StartDateTime > DateTime.Now && e.Status != EventStatus.Cancelled)
            .OrderBy(e => e.StartDateTime.Date)
            .ThenBy(e => e.StartDateTime.TimeOfDay)
            .Select(e => new EventViewModel(e, teacher, UnregisterFromEvent, EditEvent))
            .ToList();
    }

    /// <summary>
    /// Unregisters the teacher from an event.
    /// </summary>
    /// <param name="evt">The event to unregister from.</param>
    /// <returns>The async task.</returns>
    private async Task UnregisterFromEvent(Event evt)
    {
        try
        {
            var teacher = (Teacher)LoggedInUser;
            await Service.UnregisterUserFromEvent(teacher, evt);

            await LoadTeacherEventsAsync(teacher);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error unregistering from event: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles editing an event. Opens the edit dialog and updates the event if changes are made.
    /// </summary>
    /// <param name="evt">The event to edit.</param>
    /// <returns>The async task.</returns>
    private Task EditEvent(Event evt)
    {
        var teacher = (Teacher)LoggedInUser;

        var editVM = new EditEventViewModel(evt, teacher, Service, MainWindow);
        var editView = new Views.TeacherViews.EditEventView
        {
            DataContext = editVM
        };
        SetCurrentView(editView);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Navigates back to the Events menu.
    /// </summary>
    /// <returns>The async task.</returns>
    private void GoBackToEventMenu()
    {
        var view = new Views.TeacherViews.TeacherEventsMenuView
        {
            DataContext = new TeacherEventsMenuViewModel((Teacher)LoggedInUser, Service, MainWindow)
        };
        SetCurrentView(view);
    }
}

/// <summary>
/// Wrapper for Event objects that adds UI-friendly properties
/// </summary>
public class EventViewModel : IComparable<EventViewModel>
{
    private readonly Event _event;
    private readonly Teacher _teacher;
    private readonly Func<Event, Task> _unregisterFromEvent;
    private readonly Func<Event, Task> _editEvent;

    public ICommand UnregisterCommand { get; }
    public ICommand EditCommand { get; }

    /// <summary>
    /// Initializes the EventViewModel.
    /// </summary>
    /// <param name="evt">The event to display.</param>
    /// <param name="teacher">The teacher to display the event for.</param>
    /// <param name="unregisterFromEvent">The function to unregister the teacher from the event.</param>
    /// <param name="editEvent">The function to edit the event.</param>
    public EventViewModel(Event evt, Teacher teacher, Func<Event, Task> unregisterFromEvent, Func<Event, Task> editEvent)
    {
        _event = evt ?? throw new ArgumentNullException(nameof(evt));
        _teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));
        _unregisterFromEvent = unregisterFromEvent ?? throw new ArgumentNullException(nameof(unregisterFromEvent));
        _editEvent = editEvent ?? throw new ArgumentNullException(nameof(editEvent));

        UnregisterCommand = new AsyncRelayCommand(Unregister);
        EditCommand = new AsyncRelayCommand(Edit);
    }

    // Pass-through properties
    public string Title => _event.Title;
    public DateTime StartDateTime => _event.StartDateTime;
    public DateTime EndDateTime => _event.EndDateTime;
    public string RoomNumber => _event.Room.RoomNumber;
    public string ParticipantCount => $"{_event.Participants.Count}/{_event.Room.NumberOfSeats}";

    // Helper properties
    public string TypeName => _event.GetType().Name;
    public string DateText => StartDateTime.ToString("MMM d, yyyy");
    public string TimeText => StartDateTime.ToString("h:mm tt");

    public DateTime SortableDate => StartDateTime.Date;
    public TimeSpan SortableTime => StartDateTime.TimeOfDay;

    /// <summary>
    /// Check if teacher is the speaker/host of this event
    /// </summary>
    public bool IsTeacherSpeaker
    {
        get
        {
            if (_event is Conference conf)
                return conf.Speaker?.PersonId == _teacher.PersonId;
            if (_event is OfficeMeeting meeting)
                return meeting.Teacher?.PersonId == _teacher.PersonId;
            return false;
        }
    }

    /// <summary>
    /// Check if teacher can unregister from this event (not the speaker/host)
    /// </summary>
    public bool CanUnregister => !IsTeacherSpeaker;

    /// <summary>
    /// Check if teacher can edit this event (is the speaker/host)
    /// </summary>
    public bool CanEdit => IsTeacherSpeaker;

    /// <summary>
    /// Compares events by start date.
    /// </summary>
    /// <param name="other">The other event to compare to.</param>
    /// <returns>The comparison result.</returns>
    public int CompareTo(EventViewModel? other)
    {
        if (other == null) return 1;
        return StartDateTime.CompareTo(other.StartDateTime);
    }

    /// <summary>
    /// Unregisters the teacher from the event.
    /// </summary>
    /// <returns>The async task.</returns>
    public async Task Unregister()
    {
        await _unregisterFromEvent(_event);
    }

    /// <summary>
    /// Opens the edit window for the event.
    /// </summary>
    /// <returns>The async task.</returns>
    public async Task Edit()
    {
        await _editEvent(_event);
    }
}