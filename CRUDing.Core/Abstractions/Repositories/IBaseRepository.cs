using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDing.Core.Abstractions.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        public Task<TEntity?> GetByIdAsync(int id);
        public IQueryable<TEntity> GetAll();

        public Task AddAsync(TEntity entity);
        public Task RemoveAsync(TEntity entity);
        public Task UpdateAsync(TEntity entity);
        public Task RemoveRangeAsync(ICollection<TEntity> entity);
        public void Add(TEntity entity);
        public void Remove(TEntity entity);
        public void RemoveRange(ICollection<TEntity> entity);
        public void Update(TEntity entity);
        public Task SaveChangesAsync();
    }
}
