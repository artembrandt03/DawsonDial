using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Models.Enums;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;

namespace DawsonDialGUI.ViewModels.TeacherViewModels
{
    /// <summary>
    /// ViewModel for setting office hours.
    /// </summary>
    public partial class SetOfficeHoursViewModel : ViewModelBase
    {
        // Constants
        private const int DEFAULT_END_DATE_DAYS = 7;

        /// Backing fields
        public ObservableCollection<OfficeHourSlotViewModel> Slots { get; } = new();

        /// Observable Properties
        [ObservableProperty] private string errorMessage = string.Empty;
        [ObservableProperty] private string successMessage = string.Empty;
        [ObservableProperty] private DateTimeOffset? startDate = DateTimeOffset.Now; // nullable to unset on ui
        [ObservableProperty] private DateTimeOffset? endDate = DateTimeOffset.Now.AddDays(DEFAULT_END_DATE_DAYS); // same as ^
        [ObservableProperty] private bool isDropIn;

        // Relay commands
        public IRelayCommand SubmitCommand { get; }
        public IRelayCommand CancelCommand { get; }
        public IRelayCommand GoBackToSchedulingCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetOfficeHoursViewModel"/> class.
        /// </summary>
        /// <param name="teacher">The teacher viewing the view</param>
        /// <param name="service">The Dawson manager service</param>
        /// <param name="mainWindow">The main avalonia window</param>
        public SetOfficeHoursViewModel(Teacher teacher, DawsonDialService service, Window mainWindow)
        {
            Initialize(teacher, service, mainWindow);
            foreach (var day in Enum.GetValues<DayOfWeek>())
            {
                if (day is >= DayOfWeek.Monday and <= DayOfWeek.Friday)
                    Slots.Add(new OfficeHourSlotViewModel(day));
            }

            SubmitCommand = new AsyncRelayCommand(SetOfficeHoursAsync);
            CancelCommand = new RelayCommand(GoBackToMenu);
            GoBackToSchedulingCommand = new RelayCommand(GoBackToScheduling);
        }

        /// <summary>
        /// Sets the office hours for the teacher.
        /// </summary>
        /// <returns></returns>
        private async Task SetOfficeHoursAsync()
        {
            try
            {
                if (!ValidateTeacher())
                    return;

                var timeSlots = CreateTimeSlots();

                if (StartDate is null || EndDate is null)
                {
                    ErrorMessage = "Start and end dates must be selected.";
                    return;
                }

                if (!ValidateDates(StartDate.Value.DateTime, EndDate.Value.DateTime))
                    return;

                if (!timeSlots.Any())
                {
                    ErrorMessage = "At least one valid time slot must be configured.";
                    return;
                }

                await CreateOfficeHoursAsync(timeSlots);
                SuccessMessage = "Office hours set successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Validates that the current user is a teacher with an office
        /// </summary>
        private bool ValidateTeacher()
        {
            if (LoggedInUser is not Teacher teacher)
            {
                ErrorMessage = "Only teachers can set office hours.";
                return false;
            }

            if (teacher.OfficeRoom is null)
            {
                ErrorMessage = "You must be assigned an office room to set office hours.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a dictionary of time slots from the UI
        /// </summary>
        private Dictionary<DayOfWeek, (TimeOnly, TimeOnly)> CreateTimeSlots()
        {
            return Slots
                .Where(s => s.IsEnabled && s.StartTime < s.EndTime)
                .ToDictionary(
                    s => s.Day,
                    s => (TimeOnly.FromTimeSpan(s.StartTime), TimeOnly.FromTimeSpan(s.EndTime))
                );
        }

        /// <summary>
        /// Creates and saves office hours for the teacher
        /// </summary>
        /// <param name="timeSlots">The time slots to create</param>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task CreateOfficeHoursAsync(Dictionary<DayOfWeek, (TimeOnly, TimeOnly)> timeSlots)
        {
            if (LoggedInUser is not Teacher teacher)
                return;

            var startDateOnly = DateOnly.FromDateTime(StartDate!.Value.DateTime);
            var endDateOnly = DateOnly.FromDateTime(EndDate!.Value.DateTime);

            // Determine earlier and latest time slots
            var earliest = timeSlots.MinBy(kv => kv.Value.Item1);
            var latest = timeSlots.MaxBy(kv => kv.Value.Item2);

            // Create DateTime objects for start and end
            var startDateTime = ToUtc(CombineDateAndTime(startDateOnly, earliest.Value.Item1));
            var endDateTime = ToUtc(CombineDateAndTime(endDateOnly, latest.Value.Item2));

            // Create Schedule object
            var schedule = new Schedule(timeSlots, startDateOnly, endDateOnly);

            // Add schedule to the teacher's schedules
            await Service.AddScheduleAsync(schedule);

            // Create office hours
            var office = teacher.OfficeRoom!;
            var officeHours = new OfficeHours(
                $"{LoggedInUser.FirstName} {LoggedInUser.LastName}'s Office Hours",
                $"Office hours in {office.RoomNumber}",
                office,
                startDateTime,
                endDateTime,
                true,
                EventStatus.Confirmed,
                teacher,
                IsDropIn,
                schedule
            );
            officeHours.ScheduleId = schedule.ScheduleId; // Ensure ScheduleId is set
            await Service.SetTeacherOfficeHours(teacher, officeHours);
        }

        /// <summary>
        /// Validates the start and end dates.
        /// </summary>
        /// <param name="startDate">The start date to validate</param>
        /// <param name="endDate">The end date to validate</param>
        /// <returns>True if dates are valid, false otherwise</returns>
        private bool ValidateDates(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                ErrorMessage = "Start date must be before end date.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Helper method to combine data and time into a DateTime
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="time">The time.</param>
        /// <returns>The combined date time</returns>
        private DateTime CombineDateAndTime(DateOnly date, TimeOnly time) => date.ToDateTime(time);

        /// <summary>
        /// Helper method to convert dates to UTC
        /// </summary>
        /// <param name="local">Local time</param>
        /// <returns>The DateTime in UTC</returns>
        private DateTime ToUtc(DateTime local) => DateTime.SpecifyKind(local, DateTimeKind.Utc);

        /// <summary>
        /// Navigates back to the Scheduling menu.
        /// </summary>
        private void GoBackToScheduling()
        {
            var view = new Views.TeacherViews.TeacherSchedulingMenuView
            {
                DataContext = new TeacherSchedulingMenuViewModel((Teacher)LoggedInUser, Service, MainWindow)
            };
            SetCurrentView(view);
        }
    }
}
