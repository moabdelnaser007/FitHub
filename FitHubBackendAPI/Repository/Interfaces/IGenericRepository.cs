using System.Linq.Expressions;

namespace FitHubBackendAPI.Repository.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        
        Task<T?> GetByIdAsync(int id);
        Task<IQueryable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> SoftDelete(T entity);
        Task<bool> SaveChangesAsync();
        Task<bool> IsExist(int id);
    }
}
