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

namespace DawsonDialGUI.ViewModels.StudentViewModels
{
    /// <summary>
    /// ViewModel for displaying a student's upcoming events.
    /// </summary>
    public partial class StudentViewMyEventsViewModel : ViewModelBase
    {        
        [ObservableProperty] private ObservableCollection<ConferenceViewModel> upcomingEvents = new();
        [ObservableProperty] private string pageTitle = string.Empty;

        public IRelayCommand RefreshEventsCommand { get; }

        private readonly Student _student;

        /// <summary>
        /// Initializes a new instance of the StudentViewMyEventsViewModel class.
        /// </summary>
        /// <param name="student">The student viewing their events.</param>
        /// <param name="service">The service instance.</param>
        /// <param name="mainWindow">The main window instance.</param>
        public StudentViewMyEventsViewModel(Student student, DawsonDialService service, Window mainWindow)
        {
            Initialize(student, service, mainWindow);
            _student = student;

            RefreshEventsCommand = new RelayCommand(RefreshEvents);

            LoadStudentEventsAsync();
        }

        /// <summary>
        /// Loads the student's events from the database.
        /// </summary>
        private async void LoadStudentEventsAsync()
        {
            try
            {
                // Get all available conferences
                var availableConferences = await Service.GetAvailableConferences();
                
                // Filter to show only upcoming non-cancelled conferences
                var conferences = availableConferences
                    .Where(e => e.StartDateTime > DateTime.Now && e.Status != EventStatus.Cancelled)
                    .OrderBy(e => e.StartDateTime.Date)
                    .ThenBy(e => e.StartDateTime.TimeOfDay)
                    .Select(e => new ConferenceViewModel(e, _student, UnregisterFromEvent, RegisterForEvent))
                    .ToList();

                UpcomingEvents = new ObservableCollection<ConferenceViewModel>(conferences);
                PageTitle = "Available Conferences";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading conferences: {ex.Message}");
            }
        }

        /// <summary>
        /// Unregisters the student from an event.
        /// </summary>
        /// <param name="conference">The conference to unregister from.</param>
        /// <returns>The async task.</returns>
        private async Task UnregisterFromEvent(Conference conference)
        {
            try
            {
                await Service.UnregisterUserFromEvent(_student, conference);
                LoadStudentEventsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unregistering from conference: {ex.Message}");
            }
        }

        /// <summary>
        /// Registers the student for an event.
        /// </summary>
        /// <param name="conference">The conference to register for.</param>
        /// <returns>The async task.</returns>
        private async Task RegisterForEvent(Conference conference)
        {
            try
            {
                await Service.RegisterUserForEvent(_student, conference);
                LoadStudentEventsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering for conference: {ex.Message}");
            }
        }

        /// <summary>
        /// Refreshes the events list.
        /// </summary>
        private void RefreshEvents()
        {
            LoadStudentEventsAsync();
        }
    }
}
