namespace DawsonDial.Models.Events
{
    public class TimeSlot
    {
        public Guid TimeSlotId { get; set; }
        public Guid ScheduleId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public virtual Schedule Schedule { get; set; } = null!;

        public string DayOfWeekString => DayOfWeek.ToString();
        public string StartTimeString => StartTime.ToString(@"hh\:mm tt");
        public string EndTimeString => EndTime.ToString(@"hh\:mm tt");
    }
}
