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

namespace DawsonDialGUI.ViewModels.TeacherViewModels
{
    /// <summary>
    /// ViewModel for displaying all available events.
    /// </summary>
    public partial class ViewAllEventsViewModel : ViewModelBase
    {
        [ObservableProperty] private ObservableCollection<AllEventViewModel> availableEvents = new();
        [ObservableProperty] private string pageTitle = "All Upcoming Events";

        public IRelayCommand GoBackToEventMenuCommand { get; }
        public IRelayCommand RefreshEventsCommand { get; }

        /// <summary>
        /// Initializes the ViewModel.
        /// </summary>
        private ViewAllEventsViewModel()
        {
            GoBackToEventMenuCommand = new RelayCommand(GoBackToEventMenu);
            RefreshEventsCommand = new RelayCommand(RefreshEvents);
        }

        /// <summary>
        /// Refreshes the events.
        /// </summary>
        private async void RefreshEvents()
        {
            await LoadAllEventsAsync((Teacher)LoggedInUser);
        }

        /// <summary>
        /// Factory method to create and initialize the ViewModel asynchronously.
        /// </summary>
        /// <param name="teacher">The teacher to load the events for.</param>
        /// <param name="service">The service to use for the events.</param>
        /// <param name="mainWindow">The main window to use for the events.</param>
        /// <returns>The ViewModel.</returns>
        public static async Task<ViewAllEventsViewModel> CreateAsync(Teacher teacher, DawsonDialService service, Window mainWindow)
        {
            var vm = new ViewAllEventsViewModel();
            vm.Initialize(teacher, service, mainWindow);
            await vm.LoadAllEventsAsync(teacher);
            return vm;
        }

        /// <summary>
        /// Loads all available events from the database, filtered to show only upcoming ones.
        /// </summary>
        /// <param name="teacher">The teacher to load the events for.</param>
        private async Task LoadAllEventsAsync(Teacher teacher)
        {
            // Mark all past events as completed
            await Service.MarkPastEventsAsCompleted();

            // Get all upcoming and not-cancelled events
            var events = await Service.GetAllEventsAsync();

            // Filter and transform events
            var filteredEvents = FilterAndTransformEvents(events, teacher);

            AvailableEvents = new ObservableCollection<AllEventViewModel>(filteredEvents);
        }

        /// <summary>
        /// Filters events to show only upcoming ones that the teacher can join
        /// and transforms them into view models
        /// </summary>
        /// <param name="events">All events from the database</param>
        /// <param name="teacher">The teacher viewing the events</param>
        /// <returns>Filtered and ordered list of event view models</returns>
        private List<AllEventViewModel> FilterAndTransformEvents(IEnumerable<Event> events, Teacher teacher)
        {
            return events
                .Where(e => e.GetType() != typeof(OfficeHours))
                .Where(e => e.StartDateTime > DateTime.Now && e.Status != EventStatus.Cancelled && !e.IsFull)
                .Where(e => !(
                    (e.Participants.Contains(teacher)) ||
                    (e is Conference conf && conf.Speaker?.PersonId == teacher.PersonId) ||
                    (e is OfficeMeeting meeting && meeting.Teacher?.PersonId == teacher.PersonId)
                ))
                .OrderBy(e => e.StartDateTime.Date)
                .ThenBy(e => e.StartDateTime.TimeOfDay)
                .Select(e => new AllEventViewModel(e, teacher, RegisterForEvent))
                .ToList();
        }

        /// <summary>
        /// Registers the teacher for an event.
        /// </summary>
        /// <param name="evt">The event to register for.</param>
        /// <returns>The async task.</returns>
        private async Task RegisterForEvent(Event evt)
        {
            try
            {
                var teacher = (Teacher)LoggedInUser;
                await Service.RegisterUserForEvent(teacher, evt);
                await LoadAllEventsAsync(teacher);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering for event: {ex.Message}");
            }
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
    /// Wrapper for Event objects that adds UI-friendly properties specifically for the All Events view
    /// </summary>
    public class AllEventViewModel : IComparable<AllEventViewModel>
    {
        private readonly Event _event;
        private readonly Teacher _teacher;
        private readonly Func<Event, Task> _registerForEvent;

        public ICommand RegisterCommand { get; }

        /// <summary>
        /// Initializes the AllEventViewModel.
        /// </summary>
        /// <param name="evt">The event to display.</param>
        /// <param name="teacher">The teacher to display the event for.</param>
        /// <param name="registerForEvent">The function to register the teacher for the event.</param>
        public AllEventViewModel(Event evt, Teacher teacher, Func<Event, Task> registerForEvent)
        {
            _event = evt ?? throw new ArgumentNullException(nameof(evt));
            _teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));
            _registerForEvent = registerForEvent ?? throw new ArgumentNullException(nameof(registerForEvent));

            RegisterCommand = new AsyncRelayCommand(Register);
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
        public bool IsTeacherRegistered => _event.HasParticipant(_teacher);

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
        /// Check if teacher can register for this event (not already registered, not the speaker/host, and event not full)
        /// </summary>
        public bool CanRegister => !IsTeacherRegistered && !IsTeacherSpeaker && !_event.IsFull;

        /// <summary>
        /// Compares events by start date.  
        /// </summary>
        /// <param name="other">The other event to compare to.</param>
        /// <returns>The comparison result.</returns>
        public int CompareTo(AllEventViewModel? other)
        {
            if (other == null) return 1;
            return StartDateTime.CompareTo(other.StartDateTime);
        }

        /// <summary>
        /// Registers the teacher for the event.
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task Register()
        {
            await _registerForEvent(_event);
        }
    }
}