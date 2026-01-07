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

namespace DawsonDialGUI.ViewModels.StudentViewModels;

/// <summary>
/// ViewModel for viewing the student's schedule.
/// </summary>
public partial class ViewMyScheduleViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<TimeSlot> timeSlots = new();

    [ObservableProperty]    private string dateRangeText = string.Empty;
    public new ICommand GoBackToMenuCommand { get; }

    private ViewMyScheduleViewModel()
    {
        GoBackToMenuCommand = new RelayCommand(GoBackToMenu);
    }

    /// <summary>
    /// Factory method to create and initialize the ViewModel asynchronously.
    /// </summary>
    public static async Task<ViewMyScheduleViewModel> CreateAsync(Student student, DawsonDialService service, Window mainWindow)
    {
        var vm = new ViewMyScheduleViewModel();
        vm.Initialize(student, service, mainWindow);
        await vm.LoadStudentScheduleAsync(student.PersonId, service);
        return vm;
    }

    /// <summary>
    /// Loads the student's full schedule from the database and combines all time slots.
    /// </summary>
    private async Task LoadStudentScheduleAsync(Guid studentId, DawsonDialService service)
    {
        var student = await service.GetStudentWithSchedulesAsync(studentId);

        // Collect and sort time slots
        var timeSlots = CollectTimeSlots(student);

        TimeSlots = new ObservableCollection<TimeSlot>(timeSlots);

        // Update date range display
        UpdateDateRangeText(timeSlots);
    }

    /// <summary>
    /// Collects all time slots from the student's schedules and sorts them
    /// </summary>
    /// <param name="student">The student whose schedules to collect</param>
    /// <returns>A sorted list of time slots</returns>
    private List<TimeSlot> CollectTimeSlots(Student student)
    {
        var timeSlots = new List<TimeSlot>();

        // Add all class schedules
        foreach (var section in student.Classes)
            if (section.Schedule != null)
                timeSlots.AddRange(section.Schedule.TimeSlots);

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
    }    /// <summary>
    /// Navigates back to the Student menu.
    /// </summary>
    private new void GoBackToMenu()
    {
        var view = new Views.StudentViews.StudentMenuView
        {
            DataContext = new StudentMenuViewModel((Student)LoggedInUser, Service, MainWindow)
        };
        SetCurrentView(view);
    }
}
