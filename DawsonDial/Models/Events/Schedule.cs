using System.Diagnostics.CodeAnalysis;
using DawsonDial.Repositories.Interfaces;

namespace DawsonDial.Models.Events
{
    /// <summary>
    /// Represents a schedule for events.
    /// </summary>
    public class Schedule : IAggregateRoot
    {
        // Schedule Properties
        public virtual Guid ScheduleId { get; set; } = Guid.NewGuid();
        public virtual ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public virtual DateOnly StartDate { get; set;  }
        public virtual DateOnly EndDate { get; set;  }
        public byte[]? RowVersion { get; set; }

        [SetsRequiredMembers]
        protected Schedule() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// </summary>
        /// <param name="timeSlots">The time slots for each day of the week.</param>
        /// <param name="startDate">The start date of the schedule.</param>
        /// <param name="endDate">The end date of the schedule.</param>
        /// <exception cref="ArgumentException">Thrown when the time slots are invalid.</exception>
        public Schedule(Dictionary<DayOfWeek, (TimeOnly StartTime, TimeOnly EndTime)> timeSlots, DateOnly startDate, DateOnly endDate)
        {
            if (timeSlots == null || timeSlots.Count == 0)
                throw new ArgumentException("At least one day must have a time slot.", nameof(timeSlots));

            foreach (var (day, times) in timeSlots)
            {
                if (times.StartTime >= times.EndTime)
                    throw new ArgumentException($"Start time must be earlier than end time for {day}.", nameof(timeSlots));
                TimeSlots.Add(new TimeSlot
                {
                    DayOfWeek = day,
                    StartTime = times.StartTime,
                    EndTime = times.EndTime,
                    ScheduleId = ScheduleId
                });
            }
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be earlier than end date.", nameof(startDate));
            }
            StartDate = startDate;
            EndDate = endDate;
        }

        // Schedule Methods
        // TODO: Write toString method to print schedule and start date.
    }
}
