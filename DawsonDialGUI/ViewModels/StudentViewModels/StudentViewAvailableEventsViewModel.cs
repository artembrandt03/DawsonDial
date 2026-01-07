using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels
{
    public partial class StudentViewAvailableEventsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<Conference> availableConference = new();

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool hasError = false;

        private readonly Student _student;

        public StudentViewAvailableEventsViewModel(Student student, DawsonDialService service, Window mainWindow)
        {
            Initialize(student, service, mainWindow);
            _student = student;
            LoadAvailableEventsAsync();
        }

        private async void LoadAvailableEventsAsync()
        {
            try
            {
                // Students can only view available Conferences
                var conferences = await Service.GetAvailableConferences();
                AvailableConference = new ObservableCollection<Conference>(conferences);
                
                if (conferences.Count == 0)
                {
                    ErrorMessage = "No events available at this time.";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading events: {ex.Message}";
                HasError = true;
            }
        }        

        [RelayCommand]
        private async Task RegisterForConference(Conference conference)
        {
            if (conference == null)
            {
                ErrorMessage = "Invalid conference selected";
                HasError = true;
                return;
            }

            try
            {
                await Service.RegisterUserForEvent(_student, conference);
                await Task.Run(LoadAvailableEventsAsync);
                ErrorMessage = "Successfully registered for the conference!";
                HasError = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to register for conference: {ex.Message}";
                HasError = true;
            }
        }
    }
}