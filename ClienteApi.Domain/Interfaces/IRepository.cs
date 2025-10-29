using System.Linq.Expressions;

namespace ClienteApi.Domain.Interfaces
{
   
    public interface IRepository<TEntity> where TEntity : class
    {
        
        Task<TEntity?> GetByIdAsync(int id);

        
        Task<IEnumerable<TEntity>> GetAllAsync();

      
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        
        Task AddAsync(TEntity entity);

      
        void Update(TEntity entity);

       
        void Remove(TEntity entity);

       
        void RemoveRange(IEnumerable<TEntity> entities);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
