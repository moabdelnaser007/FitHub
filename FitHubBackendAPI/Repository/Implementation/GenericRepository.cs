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
            return await _db.Where(predicate).ToListAsync();
        }

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
