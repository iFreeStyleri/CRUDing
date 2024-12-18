using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Core.Abstractions.Repositories;
using CRUDing.Domain.Common.Entities;
using CRUDing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRUDing.DAL.Repositories
{
    public sealed class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : Entity
    {
        private readonly CRUDContext _context;
        public BaseRepository(CRUDContext context)
        {
            _context = context;
        }

        public async Task<TEntity> GetByIdAsync(int id)
            => await _context
                .Set<TEntity>()
                .SingleOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

        public IQueryable<TEntity> GetAll()
            => _context.Set<TEntity>();

        public async Task AddAsync(TEntity entity)
        {
            _context.Add(entity);
            await SaveChangesAsync();
        }

        public async Task RemoveAsync(TEntity entity)
        {
            _context.Remove(entity);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Update(entity);
            await SaveChangesAsync();
        }

        public async Task RemoveRangeAsync(ICollection<TEntity> entity)
        {
            _context.RemoveRange(entity);
            await SaveChangesAsync();
        }

        public void Add(TEntity entity)
        {
            _context.Add(entity);
        }

        public void Remove(TEntity entity)
        {
            _context.Remove(entity);
        }

        public void RemoveRange(ICollection<TEntity> entity)
        {
            _context.RemoveRange(entity);
        }

        public void Update(TEntity entity)
        {
            _context.Update(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
