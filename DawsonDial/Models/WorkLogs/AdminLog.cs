using DawsonDial.Repositories.Interfaces;

namespace DawsonDial.Models.People
{
    // Implements IAggregateRoot to enable usage with generic IRepository<T> pattern
    public class AdminLog : IAggregateRoot
    {
        public Guid AdminLogId { get; set; } = Guid.NewGuid();
        public string Action { get; set; } = null!;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public Guid AdminId { get; set; }
        public Admin Admin { get; set; } = null!;
    }
}