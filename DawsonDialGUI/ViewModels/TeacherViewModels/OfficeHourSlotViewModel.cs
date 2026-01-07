using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DawsonDialGUI.ViewModels.TeacherViewModels
{
    /// <summary>
    /// Represents a view model for an office hour slot.
    /// </summary>
    public partial class OfficeHourSlotViewModel : ObservableObject
    {
        // Constants
        private const int DEFAULT_START_HOUR = 9;
        private const int DEFAULT_END_HOUR = 10;
        private const int DEFAULT_MINUTES = 0;
        private const int DEFAULT_SECONDS = 0;

        // Backing fields
        public DayOfWeek Day { get; }

        // Observable Properties
        [ObservableProperty] private bool isEnabled;

        [ObservableProperty] private TimeSpan startTime = new(DEFAULT_START_HOUR, DEFAULT_MINUTES, DEFAULT_SECONDS);

        [ObservableProperty] private TimeSpan endTime = new(DEFAULT_END_HOUR, DEFAULT_MINUTES, DEFAULT_SECONDS);

        /// <summary>
        /// Initializes a new instance of the <see cref="OfficeHourSlotViewModel"/> class.
        /// </summary>
        /// <param name="day">The day of the week</param>
        public OfficeHourSlotViewModel(DayOfWeek day)
        {
            Day = day;
        }
    }
}