using System.Linq.Expressions;

namespace FitHubBackendAPI.Repository.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        
        Task<T?> GetByIdAsync(int id);
        Task<IQueryable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // ✅✅ دي الدالة الجديدة "الجوكر" اللي هنضيفها
        // بتقبل فلتر، وبتقبل Includes (عشان نجيب جداول مرتبطة)، وبتقبل ترتيب
        Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null
        );

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> SoftDelete(T entity);
        Task<bool> SaveChangesAsync();
        Task<bool> IsExist(int id);
    }
}
