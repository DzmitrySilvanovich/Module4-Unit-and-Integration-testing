using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Contracts;
using Ticketing.DAL.Domain;

namespace Ticketing.DAL.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationContext _db;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationContext context)
        {
            _db = context;
            _dbSet = _db.Set<T>();
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            EntityEntry<T> addedEntity = await _dbSet.AddAsync(entity);

            await _db.SaveChangesAsync();
            return addedEntity.Entity;
        }

        public virtual Task<IQueryable<T>> GetAllAsync()
        {
            return Task.FromResult(_dbSet.AsQueryable());
        }

        public virtual async ValueTask<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _db.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(object id)
        {
            if (await _dbSet.FindAsync(id) is T entity)
            {
                await DeleteAsync(entity);
            }
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
