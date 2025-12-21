using FitHubBackendAPI.Data;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FitHubBackendAPI.Repository.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly FitHubDbContext _context;
        private readonly DbSet<T> _db;

        public GenericRepository(FitHubDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _db.FindAsync(id);
            return entity?.IsDeleted == false ? entity : null;
        }

        public async Task<IQueryable<T>> GetAllAsync()
        {
            return _db.Where(x => !x.IsDeleted);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            var entities = await _db.Where(predicate).ToListAsync();
            return entities.Where(x => !x.IsDeleted);
        }

        // =========================================================================
        // ✅ الدالة الجديدة (الجوكر): بتقبل فلتر، وجداول مرتبطة، وترتيب
        // =========================================================================
        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? filter = null,       // شرط البحث (اختياري)
            string? includeProperties = null,               // أسماء الجداول المرتبطة (مفصولة بفاصلة)
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null) // دالة الترتيب (اختياري)
        {
            // 1. بنعمل Query مبدئي على الجدول
            IQueryable<T> query = _db;

            // 2. لو مبعوت شرط (Filter)، بنطبقه
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // 3. لو مبعوت جداول مرتبطة (Includes)، بنلف عليهم ونعملهم Include
            // مثال: "Branch,Review" -> هيفصلهم ويعمل Include لكل واحد
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            // 4. لو مبعوت ترتيب (OrderBy)، بنرتب ونرجع الليسته
            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            // 5. لو مفيش ترتيب، بنرجع الداتا زي ما هي بعد الفلترة
            return await query.ToListAsync();
        }
        // =========================================================================

        public async Task AddAsync(T entity)
        {
            await _db.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _db.Update(entity);
        }

        public void Delete(T entity)
        {
            _db.Remove(entity);
        }
        public async Task<bool> SoftDelete(T entity)
        {
            if(entity.IsDeleted)
                return true;
            entity.IsDeleted = true;
            _db.Update(entity);
            return true;
        }


        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public Task<bool> IsExist(int id)
        {
            return _db.AnyAsync(e => e.Id == id && !e.IsDeleted);
        }
    }
}