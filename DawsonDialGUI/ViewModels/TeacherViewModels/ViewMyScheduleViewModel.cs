using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DawsonDialGUI.ViewModels.TeacherViewModels;

/// <summary>
/// ViewModel for viewing the teacher's schedule.
/// </summary>
public partial class ViewMyScheduleViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<TimeSlot> timeSlots = new();

    [ObservableProperty]
    private string dateRangeText = string.Empty;
    public ICommand GoBackToSchedulingCommand { get; }

    private ViewMyScheduleViewModel()
    {
        GoBackToSchedulingCommand = new RelayCommand(GoBackToScheduling);
    }

    /// <summary>
    /// Factory method to create and initialize the ViewModel asynchronously.
    /// </summary>
    public static async Task<ViewMyScheduleViewModel> CreateAsync(Teacher teacher, DawsonDialService service, Window mainWindow)
    {
        var vm = new ViewMyScheduleViewModel();
        vm.Initialize(teacher, service, mainWindow);
        await vm.LoadTeacherScheduleAsync(teacher.PersonId, service);
        return vm;
    }

    /// <summary>
    /// Loads the teacher's full schedule from the database and combines all time slots.
    /// </summary>
    private async Task LoadTeacherScheduleAsync(Guid teacherId, DawsonDialService service)
    {
        var teacher = await service.GetTeacherWithSchedulesAsync(teacherId);

        // Collect and sort time slots
        var timeSlots = CollectTimeSlots(teacher);

        TimeSlots = new ObservableCollection<TimeSlot>(timeSlots);

        // Update date range display
        UpdateDateRangeText(timeSlots);
    }

    /// <summary>
    /// Collects all time slots from the teacher's schedules and sorts them
    /// </summary>
    /// <param name="teacher">The teacher whose schedules to collect</param>
    /// <returns>A sorted list of time slots</returns>
    private List<TimeSlot> CollectTimeSlots(Teacher teacher)
    {
        var timeSlots = new List<TimeSlot>();

        // Add all class schedules
        foreach (var section in teacher.Classes)
            if (section.Schedule != null)
                timeSlots.AddRange(section.Schedule.TimeSlots);

        // Add office hours schedule
        if (teacher.OfficeHours?.Schedule != null)
            timeSlots.AddRange(teacher.OfficeHours.Schedule.TimeSlots);

        // Sort by day of week first, then by start time
        return timeSlots.OrderBy(ts => ts.DayOfWeek)
                        .ThenBy(ts => ts.StartTime)
                        .ToList();
    }

    /// <summary>
    /// Updates the date range text based on the available time slots
    /// </summary>
    /// <param name="timeSlots">The time slots to derive date range from</param>
    private void UpdateDateRangeText(List<TimeSlot> timeSlots)
    {
        if (timeSlots.Any())
        {
            var minDate = timeSlots.Min(ts => ts.Schedule.StartDate);
            var maxDate = timeSlots.Max(ts => ts.Schedule.EndDate);
            DateRangeText = $"{minDate:MMM d, yyyy} - {maxDate:MMM d, yyyy}";
        }
        else
        {
            DateRangeText = "No schedule available.";
        }
    }

    /// <summary>
    /// Navigates back to the Scheduling/Availability menu.
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