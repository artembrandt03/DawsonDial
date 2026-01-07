using DawsonDial.Models.People;
using DawsonDial.Models.Events;

namespace DawsonDial.Repositories.Interfaces
{
    public interface IPersonRepository : IRepository<Person>
    {
        Task<Person> GetUserByUsernameAndPasswordAsync(string username, string password);
        Task<Person> GetUserByUsernameAsync(string username);
        Task<Person> UpdateUserPasswordByUsernameAsync(string username, string password);
        Task SetOfficeHoursAsync(Teacher teacher, OfficeHours officeHours);
        Task<Teacher> GetTeacherWithSchedulesAsync(Guid teacherId);
        Task<Student> GetStudentWithSchedulesAsync(Guid studentId);
    }
}