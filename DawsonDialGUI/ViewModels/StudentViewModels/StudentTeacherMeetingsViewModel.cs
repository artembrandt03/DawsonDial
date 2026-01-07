using System;
using System.Collections.ObjectModel;
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

namespace DawsonDialGUI.ViewModels.StudentViewModels
{
    /// <summary>
    /// ViewModel for displaying a student's teacher meetings.
    /// </summary>
    public partial class StudentTeacherMeetingsViewModel : ViewModelBase
    {
        [ObservableProperty] private ObservableCollection<MeetingViewModel> teacherMeetings = new();
        [ObservableProperty] private string pageTitle = string.Empty;
        [ObservableProperty] private string errorMessage = string.Empty;

        public IRelayCommand GoBackCommand { get; }
        public IRelayCommand RefreshMeetingsCommand { get; }

        public StudentTeacherMeetingsViewModel(Student student, DawsonDialService service, Window mainWindow)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (mainWindow == null) throw new ArgumentNullException(nameof(mainWindow));

            Initialize(student, service, mainWindow);
            GoBackCommand = new RelayCommand(GoBack);
            RefreshMeetingsCommand = new RelayCommand(RefreshMeetings);

            LoadTeacherMeetings();
        }

        private void LoadTeacherMeetings()
        {
            try
            {
                var meetings = ((Student)LoggedInUser).Events
                    .Where(e => e is OfficeMeeting && e.StartDateTime > DateTime.Now && e.Status != EventStatus.Cancelled)
                    .Cast<OfficeMeeting>()
                    .OrderBy(e => e.StartDateTime)
                    .Select(m => new MeetingViewModel(m))
                    .ToList();

                TeacherMeetings = new ObservableCollection<MeetingViewModel>(meetings);
                PageTitle = $"Your Teacher Meetings";
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading meetings: {ex.Message}";
            }
        }

        private void RefreshMeetings()
        {
            LoadTeacherMeetings();
        }

        private void GoBack()
        {
            var view = new StudentMenuView
            {
                DataContext = new StudentMenuViewModel((Student)LoggedInUser!, Service, MainWindow)
            };
            SetCurrentView(view);
        }
    }    public class MeetingViewModel
    {
        public string Title { get; }
        public string TeacherName { get; }
        public string DateText { get; }
        public string TimeText { get; }
        public string RoomNumber { get; }
        public string Description { get; }
        public string TypeName => "Meeting";
        public int ParticipantCount { get; }
        public string SortableDate { get; }
        public string SortableTime { get; }

        public MeetingViewModel(OfficeMeeting meeting)
        {
            Title = meeting.Title ?? "Untitled Meeting";
            TeacherName = meeting.Teacher != null ? $"{meeting.Teacher.FirstName} {meeting.Teacher.LastName}" : "Unknown Teacher";
            DateText = meeting.StartDateTime.ToString("MM/dd/yyyy");
            TimeText = $"{meeting.StartDateTime:HH:mm} - {meeting.EndDateTime:HH:mm}";
            RoomNumber = meeting.Room?.RoomNumber ?? "No Room";
            Description = meeting.Description ?? "No description available";
            ParticipantCount = meeting.Participants?.Count ?? 0;
            SortableDate = meeting.StartDateTime.ToString("yyyy-MM-dd");
            SortableTime = meeting.StartDateTime.ToString("HH:mm");
        }
    }
}
