using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.Events;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using DawsonDialGUI.Views;
using System.Globalization;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;

namespace DawsonDialGUI.ViewModels;

public partial class AddCourseViewModel : ViewModelBase
{
    [ObservableProperty] private string courseCode = string.Empty;
    [ObservableProperty] private string subject = string.Empty;
    [ObservableProperty] private string sectionNumber = string.Empty;
    [ObservableProperty] private string semester = string.Empty;

    [ObservableProperty] private DateOnly startDate;
    [ObservableProperty] private DateOnly endDate;

    [ObservableProperty] private string successMessage = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;

    public ObservableCollection<DaySlotInput> DaySlots { get; } = new();

    [ObservableProperty]
    private ObservableCollection<Teacher> teachers = new();

    [ObservableProperty]
    private Teacher? selectedTeacher;

    public IRelayCommand SubmitCommand { get; }

    public AddCourseViewModel(Admin admin, DawsonDialService service, Window mainWindow)
    {
        Initialize(admin, service, mainWindow);
        SubmitCommand = new AsyncRelayCommand(CreateCourseWithScheduleAndSection);

        // Prepopulate days
        foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
        {
            DaySlots.Add(new DaySlotInput { Day = day });
        }

        StartDate = DateOnly.FromDateTime(DateTime.Today);
        EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7));

        LoadTeachers();
    }

    private async Task CreateCourseWithScheduleAndSection()
    {
        ErrorMessage = SuccessMessage = string.Empty;

        try
        {
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

            var schedule = new Schedule(timeSlotDict, StartDate, EndDate);
            await Service.AddSchedule(schedule); 

            var course = new Course(CourseCode, Subject, new HashSet<Teacher>(), new HashSet<Section>());
            await Service.AddCourse(course);

            var teachers = await Service.GetAllTeachers();
            var dummyTeacher = teachers.FirstOrDefault(); 
            if (dummyTeacher is null)
                throw new Exception("No teacher available to assign to section.");
            var emptyRoomMap = new Dictionary<string, Room>();
            var section = new Section(
                course,
                int.Parse(SectionNumber),
                SelectedTeacher ?? throw new Exception("You must select a teacher."),
                schedule,
                emptyRoomMap
            );

            await Service.AddSection(section);; 

            SuccessMessage = "Course, section, and schedule created successfully.";
            await Service.LogAction((Admin)LoggedInUser, $"Created course {CourseCode} with section {SectionNumber} and schedule.");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to create course: {ex.Message}";
        }
    }

    public partial class DaySlotInput : ObservableObject
    {
        [ObservableProperty] public DayOfWeek day;
        [ObservableProperty] public bool isEnabled;
        [ObservableProperty] public string startTime = "08:00";
        [ObservableProperty] public string endTime = "10:00";
    }

    protected override void GoBackToMenu()
    {
        SetCurrentView(new CourseMenuView
        {
            DataContext = new CourseMenuViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
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

    
    private async void LoadTeachers()
    {
        var allTeachers = await Service.GetAllTeachers();
        Teachers = new ObservableCollection<Teacher>(allTeachers);
    }
}
