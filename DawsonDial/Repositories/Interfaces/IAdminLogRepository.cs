using DawsonDial.Models.People;

namespace DawsonDial.Repositories.Interfaces
{
    public interface IAdminLogRepository : IRepository<AdminLog>
    {
        Task<IEnumerable<AdminLog>> GetLogsByAdminIdAsync(Guid adminId);
    }
}