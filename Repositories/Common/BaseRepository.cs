using Microsoft.EntityFrameworkCore;
using NucpaBalloonsApi.Interfaces.Repositories.Common;
using NucpaBalloonsApi.Models.Common;

namespace NucpaBalloonsApi.Repositories.Common
{
    public class BaseRepository<T>(NucpaDbContext context) : IBaseRepository<T> where T : BaseEntity
    {
        private readonly NucpaDbContext _context = context
            ?? throw new ArgumentNullException(nameof(context));
        private readonly DbSet<T> _dbSet = context.Set<T>();

        public virtual async Task<T> InsertAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(string id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(string id)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            var existingEntity = await _dbSet.FindAsync(entity.Id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                _context.Entry(existingEntity).State = EntityState.Modified;
            }
            else
            {
                await _dbSet.AddAsync(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return entity;
        }


        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}