using IAndOthers.Core.Data.Entity;
using IAndOthers.Core.Data.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace IAndOthers.Core.Data.Services
{
    public interface IIORepository<TEntity, TContext> where TEntity : class, IIOEntity, new() where TContext : DbContext, new()
    {
        Task<IOResult<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);
        Task<IOResult<IList<TEntity>>> GetListAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes);

        Task<IOResultMetadata> InsertAsync(TEntity entity, IOIdentityBase identity = null);
        Task<IOResultMetadata> InsertAsync(IEnumerable<TEntity> entities, IOIdentityBase identity = null);

        Task<IOResultMetadata> UpdateAsync(TEntity entity, IOIdentityBase identity = null);
        Task<IOResultMetadata> UpdateAsync(IEnumerable<TEntity> entities, IOIdentityBase identity = null);

        Task<IOResultMetadata> DeleteAsync(TEntity entity, IOIdentityBase identity = null);
        Task<IOResultMetadata> DeleteAsync(long id, IOIdentityBase identity = null);
        Task<IOResultMetadata> DeleteAsync(IEnumerable<TEntity> entities, IOIdentityBase identity = null);

        TContext Context { get; set; }
        IQueryable<TEntity> Table { get; }
    }
}
