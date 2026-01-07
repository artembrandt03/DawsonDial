namespace DawsonDial.Repositories.Interfaces
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }

    public interface IAggregateRoot { }
}
