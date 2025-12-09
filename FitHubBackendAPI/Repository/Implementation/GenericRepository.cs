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
            return await _db.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _db.ToListAsync();
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

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
