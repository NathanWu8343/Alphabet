using Microsoft.EntityFrameworkCore;
using SharedKernel.Core;

namespace UrlShortener.Persistence.Repositories
{
    internal abstract class Repository<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
        where TKey : class
    {
        protected readonly ApplicationDbContext DbContext;

        protected Repository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual Task<TEntity?> GetByIdAsync(TKey id)
        {
            return DbContext.Set<TEntity>()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public virtual void Add(TEntity entity)
        {
            DbContext.Set<TEntity>().Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            DbContext.Set<TEntity>().Update(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            DbContext.Set<TEntity>().Remove(entity);
        }
    }
}