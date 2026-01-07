using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;
using DawsonDial.Services.ManagerService;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels;

public partial class ModifyCourseViewModel : ViewModelBase
{
    [ObservableProperty] private string courseCode;
    [ObservableProperty] private string subject;
    [ObservableProperty] private string sectionNumber;

    [ObservableProperty] private DateOnly startDate;
    [ObservableProperty] private DateOnly endDate;

    [ObservableProperty] private string successMessage = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;

    [ObservableProperty] private ObservableCollection<Teacher> teachers = new();
    [ObservableProperty] private Teacher? selectedTeacher;

    public string Semester
    {
        get
        {
            var month = originalSection.Schedule.StartDate.Month;
            var year = originalSection.Schedule.StartDate.Year;

            return month switch
            {
                <= 4 => $"Winter {year}",
                <= 8 => $"Summer {year}",
                _ => $"Fall {year}"
            };
        }
    }

    public ObservableCollection<AddCourseViewModel.DaySlotInput> DaySlots { get; } = new();

    private readonly Course course;
    private readonly Section originalSection;

    public IRelayCommand SubmitCommand { get; }

    public ModifyCourseViewModel(Admin admin, DawsonDialService service, Window mainWindow, Course course)
    {
        Initialize(admin, service, mainWindow);

        this.course = course ?? throw new ArgumentNullException(nameof(course));
        SubmitCommand = new AsyncRelayCommand(ModifyCourseAsync);

        courseCode = course.CourseCode;
        subject = course.Subject;

        originalSection = course.Sections.FirstOrDefault();
        if (originalSection == null)
        {
            ErrorMessage = "This course has no sections to modify.";
            return; // Stop early
        }


        sectionNumber = originalSection.SectionNumber.ToString();

        startDate = originalSection.Schedule.StartDate;
        endDate = originalSection.Schedule.EndDate;

        foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
        {
            var slot = originalSection.Schedule.TimeSlots.FirstOrDefault(ts => ts.DayOfWeek == day);
            DaySlots.Add(new AddCourseViewModel.DaySlotInput
            {
                Day = day,
                IsEnabled = slot != null,
                StartTime = slot?.StartTime.ToString("HH:mm") ?? "08:00",
                EndTime = slot?.EndTime.ToString("HH:mm") ?? "10:00"
            });
        }

        _ = LoadTeachers();
    }

    private async Task LoadTeachers()
    {
        var allTeachers = await Service.GetAllTeachers();
        Teachers = new ObservableCollection<Teacher>(allTeachers);
        SelectedTeacher = originalSection.Teacher;
    }

    private async Task ModifyCourseAsync()
    {
        ErrorMessage = SuccessMessage = string.Empty;

        try
        {
            // Update course basic fields
            course.CourseCode = CourseCode;
            course.Subject = Subject;

            // Parse section number
            if (!int.TryParse(SectionNumber, out var parsedSectionNumber))
                throw new Exception("Section number must be a valid integer.");

            // Build new schedule
            var timeSlotDict = new Dictionary<DayOfWeek, (TimeOnly Start, TimeOnly End)>();

            foreach (var slot in DaySlots)
            {
                if (slot.IsEnabled)
                {
                    if (!TimeOnly.TryParseExact(slot.StartTime, "HH:mm", out var start))
                        throw new Exception($"Invalid start time for {slot.Day}");
                    if (!TimeOnly.TryParseExact(slot.EndTime, "HH:mm", out var end))
                        throw new Exception($"Invalid end time for {slot.Day}");
                    timeSlotDict[slot.Day] = (start, end);
                }
            }

            var updatedSchedule = new Schedule(timeSlotDict, StartDate, EndDate);
            await Service.AddSchedule(updatedSchedule);

            // Update section properties
            originalSection.Schedule = updatedSchedule;
            originalSection.Teacher = SelectedTeacher ?? throw new Exception("A teacher must be selected.");
            originalSection.SectionNumber = parsedSectionNumber;

            await Service.UpdateCourse(course);
            SuccessMessage = "Course updated successfully!";
            await Service.LogAction((Admin)LoggedInUser, $"Updated course {CourseCode} with section {SectionNumber} and schedule.");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to update course: {ex.Message}";
        }
    }

    public DateTimeOffset? StartDateUI
    {
        get => new DateTimeOffset(StartDate.ToDateTime(TimeOnly.MinValue));
        set
        {
            if (value.HasValue)
                StartDate = DateOnly.FromDateTime(value.Value.DateTime);
        }
    }

    public DateTimeOffset? EndDateUI
    {
        get => new DateTimeOffset(EndDate.ToDateTime(TimeOnly.MinValue));
        set
        {
            if (value.HasValue)
                EndDate = DateOnly.FromDateTime(value.Value.DateTime);
        }
    }

    protected override void GoBackToMenu()
    {
        SetCurrentView(new CourseMenuView
        {
            DataContext = new CourseMenuViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }
}
