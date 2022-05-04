using System.Linq.Expressions;

namespace Infrastructure.Contracts
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        bool Exists(Expression<Func<TEntity, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entity);
        void Remove(TEntity entity);
        void Update(TEntity entity);
        Task<TEntity> SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleOrDefaultNoTracking(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultNoTracking(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        void UpdateRange(IEnumerable<TEntity> entity);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);
        void RemoveRange(IEnumerable<TEntity> entity);
        IQueryable<TEntity> QueryAll(Expression<Func<TEntity, bool>> predicate = null);
        IQueryable<TEntity> FromSqlRaw(string query, object param = null);
    }
}
