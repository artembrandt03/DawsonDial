using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Models.Enums;
using DawsonDial.Services.ManagerService;
using DawsonDialGUI.Views.StudentViews;
using DawsonDialGUI.ViewModels;

namespace DawsonDialGUI.ViewModels.StudentViewModels
{
    /// <summary>
    /// ViewModel for booking a meeting with a teacher.
    /// </summary>
    public partial class BookTeacherMeetingViewModel : ViewModelBase
    {
        // Observable properties
        [ObservableProperty] private string errorMessage = string.Empty;
        [ObservableProperty] private string successMessage = string.Empty;
        [ObservableProperty] private bool isBookingEnabled = true;
        [ObservableProperty] private List<Teacher> availableTeachers = new();
        [ObservableProperty] private Teacher? selectedTeacher;
        [ObservableProperty] private string topic = string.Empty;
        [ObservableProperty] private string description = string.Empty;
        [ObservableProperty] private DateTime meetingDate = DateTime.Now.Date;
        [ObservableProperty] private TimeSpan startTime = new(9, 0, 0); // Default to 9 AM
        [ObservableProperty] private int durationMinutes = 30;

        // Commands
        public IRelayCommand BookMeetingCommand { get; }
        public IRelayCommand GoBackCommand { get; }

        public BookTeacherMeetingViewModel(Student student, DawsonDialService service, Window mainWindow)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (mainWindow == null) throw new ArgumentNullException(nameof(mainWindow));

            Initialize(student, service, mainWindow);
            BookMeetingCommand = new AsyncRelayCommand(BookMeetingAsync);
            GoBackCommand = new RelayCommand(GoBack);

            // Load teachers from student's classes
            if (student.Classes != null)
            {                AvailableTeachers = student.Classes
                    .Where(s => s?.Teacher != null)
                    .Select(s => s.Teacher!)
                    .Distinct()
                    .OrderBy(t => t.LastName ?? string.Empty)
                    .ThenBy(t => t.FirstName ?? string.Empty)
                    .ToList();
            }
        }

        partial void OnMeetingDateChanged(DateTime value)
        {
            if (value.Date < DateTime.Now.Date)
            {
                ErrorMessage = "Meeting date cannot be in the past.";
                MeetingDate = DateTime.Now.Date;
            }
            else
            {
                ErrorMessage = string.Empty;
            }
        }

        private async Task BookMeetingAsync()
        {
            if (!ValidateInputs())
                return;

            IsBookingEnabled = false;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            try
            {
                var startDateTime = MeetingDate.Date.Add(StartTime);
                var endDateTime = startDateTime.AddMinutes(DurationMinutes);

                if (SelectedTeacher?.OfficeRoom == null)
                {
                    ErrorMessage = "Selected teacher does not have an office assigned.";
                    return;
                }

                var office = SelectedTeacher.OfficeRoom;
                var meeting = new OfficeMeeting(
                    Topic,
                    Description,
                    office,
                    startDateTime.ToUniversalTime(),
                    endDateTime.ToUniversalTime(),
                    false,
                    EventStatus.Planned,
                    SelectedTeacher);

                if (LoggedInUser == null)
                {
                    ErrorMessage = "User session expired. Please log in again.";
                    return;
                }

                meeting.AddParticipant((Student)LoggedInUser);
                meeting.AddParticipant(SelectedTeacher);

                bool reserved = await Service.ReserveRoom(office, meeting, startDateTime.ToUniversalTime(), TimeSpan.FromMinutes(DurationMinutes));

                if (reserved)
                {
                    await Service.ScheduleEvent(meeting);
                    ((Student)LoggedInUser).AddEvent(meeting);
                    SelectedTeacher.AddEvent(meeting);

                    SuccessMessage = "Meeting booked successfully!";
                    await Task.Delay(2000); // Show success message briefly
                    GoBack();
                }
                else
                {
                    ErrorMessage = "Failed to book the room. It may no longer be available.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error booking meeting: {ex.Message}";
            }
            finally
            {
                IsBookingEnabled = true;
            }
        }

        private bool ValidateInputs()
        {
            if (SelectedTeacher == null)
            {
                ErrorMessage = "Please select a teacher.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Topic))
            {
                ErrorMessage = "Please enter a meeting topic.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                ErrorMessage = "Please enter a meeting description.";
                return false;
            }

            if (MeetingDate.Date < DateTime.Now.Date)
            {
                ErrorMessage = "Meeting date cannot be in the past.";
                return false;
            }

            if (DurationMinutes < 15 || DurationMinutes > 60)
            {
                ErrorMessage = "Meeting duration must be between 15 and 60 minutes.";
                return false;
            }

            if (SelectedTeacher.OfficeRoom == null)
            {
                ErrorMessage = "Selected teacher does not have an office assigned.";
                return false;
            }

            // Check if the meeting time is during business hours (8 AM to 6 PM)
            var startTime = MeetingDate.Date.Add(StartTime);
            var endTime = startTime.AddMinutes(DurationMinutes);
            var businessStart = startTime.Date.Add(new TimeSpan(8, 0, 0));
            var businessEnd = startTime.Date.Add(new TimeSpan(18, 0, 0));

            if (startTime < businessStart || endTime > businessEnd)
            {
                ErrorMessage = "Meetings must be scheduled between 8 AM and 6 PM.";
                return false;
            }

            return true;
        }

        private void GoBack()
        {
            var view = new StudentMenuView
            {
                DataContext = new StudentMenuViewModel((Student)LoggedInUser!, Service, MainWindow)
            };
            SetCurrentView(view);
        }
    }
}
