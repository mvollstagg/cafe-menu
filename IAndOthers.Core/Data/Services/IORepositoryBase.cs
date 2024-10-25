using IAndOthers.Core.Data.Entity;
using IAndOthers.Core.Data.Enumeration;
using IAndOthers.Core.Data.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace IAndOthers.Core.Data.Services
{
    public class IORepositoryBase<TEntity, TContext> : IIORepository<TEntity, TContext>
        where TEntity : class, IIOEntity, new()
        where TContext : DbContext, new()
    {
        public IORepositoryBase(TContext context)
        {
            Context = context;
        }

        public TContext Context { get; set; }

        public IQueryable<TEntity> Table => Context.Set<TEntity>();

        public virtual async Task<IOResult<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = Table;

            if (include != null)
                query = include(query);

            if (filter != null)
                query = query.Where(filter);

            var entity = await query.FirstOrDefaultAsync();
            return entity != null
                ? new IOResult<TEntity>(IOResultStatusEnum.Success, entity)
                : new IOResult<TEntity>(IOResultStatusEnum.Error, null, "Entity not found");
        }

        public virtual async Task<IOResult<IList<TEntity>>> GetListAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = Table;

            if (filter != null)
                query = query.Where(filter);

            if (includes != null)
                query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (orderBy != null)
                query = orderBy(query);

            var entities = await query.ToListAsync();
            return new IOResult<IList<TEntity>>(IOResultStatusEnum.Success, entities);
        }

        public virtual async Task<IOResultMetadata> InsertAsync(TEntity entity, IOIdentityBase identity)
        {
            try
            {
                if (entity is IOEntityTrackable trackableEntity)
                {
                    if (identity != null)
                    {
                        trackableEntity.CreatedById = identity.IdentityId;
                    }
                    trackableEntity.CreationDateUtc = DateTime.UtcNow;
                }
                await Context.Set<TEntity>().AddAsync(entity);
                await Context.SaveChangesAsync();
                return new IOResultMetadata(IOResultStatusEnum.Success, "Entity inserted successfully");
            }
            catch (Exception ex)
            {
                return new IOResultMetadata(IOResultStatusEnum.Error, ex.Message);
            }
        }

        public virtual async Task<IOResultMetadata> InsertAsync(IEnumerable<TEntity> entities, IOIdentityBase identity)
        {
            try
            {
                foreach (var entity in entities)
                {
                    if (entity is IOEntityTrackable trackableEntity)
                    {
                        if (identity != null)
                        {
                            trackableEntity.CreatedById = identity.IdentityId;
                        }
                        trackableEntity.CreationDateUtc = DateTime.UtcNow;
                    }
                }
                await Context.Set<TEntity>().AddRangeAsync(entities);
                await Context.SaveChangesAsync();
                return new IOResultMetadata(IOResultStatusEnum.Success, "Entities inserted successfully");
            }
            catch (Exception ex)
            {
                return new IOResultMetadata(IOResultStatusEnum.Error, ex.Message);
            }
        }

        public virtual async Task<IOResultMetadata> UpdateAsync(TEntity entity, IOIdentityBase identity)
        {
            try
            {
                if (entity is IOEntityTrackable trackableEntity)
                {
                    if (identity != null)
                    {
                        trackableEntity.ModificatedById = identity.IdentityId;
                    }
                    trackableEntity.ModificationDateUtc = DateTime.UtcNow;
                }
                Context.Set<TEntity>().Update(entity);
                await Context.SaveChangesAsync();
                return new IOResultMetadata(IOResultStatusEnum.Success, "Entity updated successfully");
            }
            catch (Exception ex)
            {
                return new IOResultMetadata(IOResultStatusEnum.Error, ex.Message);
            }
        }

        public virtual async Task<IOResultMetadata> UpdateAsync(IEnumerable<TEntity> entities, IOIdentityBase identity)
        {
            try
            {
                foreach (var entity in entities)
                {
                    if (entity is IOEntityTrackable trackableEntity)
                    {
                        if (identity != null)
                        {
                            trackableEntity.ModificatedById = identity.IdentityId;
                        }
                        trackableEntity.ModificationDateUtc = DateTime.UtcNow;
                    }
                }
                Context.Set<TEntity>().UpdateRange(entities);
                await Context.SaveChangesAsync();
                return new IOResultMetadata(IOResultStatusEnum.Success, "Entities updated successfully");
            }
            catch (Exception ex)
            {
                return new IOResultMetadata(IOResultStatusEnum.Error, ex.Message);
            }
        }

        public virtual async Task<IOResultMetadata> DeleteAsync(TEntity entity, IOIdentityBase identity)
        {
            try
            {
                if (entity is IOEntityDeletable deletableEntity)
                {
                    if (identity != null)
                    {
                        deletableEntity.ModificatedById = identity.IdentityId;
                    }
                    deletableEntity.ModificationDateUtc = DateTime.UtcNow;
                    deletableEntity.Deleted = true;
                }
                Context.Set<TEntity>().Update(entity);
                await Context.SaveChangesAsync();
                return new IOResultMetadata(IOResultStatusEnum.Success, "Entity deleted successfully");
            }
            catch (Exception ex)
            {
                return new IOResultMetadata(IOResultStatusEnum.Error, ex.Message);
            }
        }

        public virtual async Task<IOResultMetadata> DeleteAsync(long id, IOIdentityBase identity)
        {
            var entity = await Context.Set<TEntity>().FindAsync(id);
            if (entity != null)
            {
                return await DeleteAsync(entity, identity);
            }
            return new IOResultMetadata(IOResultStatusEnum.Error, "Entity not found");
        }

        public virtual async Task<IOResultMetadata> DeleteAsync(IEnumerable<TEntity> entities, IOIdentityBase identity)
        {
            try
            {
                foreach (var entity in entities)
                {
                    if (entity is IOEntityDeletable deletableEntity)
                    {
                        // Soft delete logic: Mark as deleted instead of removing
                        if (identity != null)
                        {
                            deletableEntity.ModificatedById = identity.IdentityId;
                        }
                        deletableEntity.ModificationDateUtc = DateTime.UtcNow;
                        deletableEntity.Deleted = true;
                    }
                    else
                    {
                        // Hard delete logic: Remove the entity from the context
                        Context.Set<TEntity>().Remove(entity);
                    }
                }

                // Only update entities that were marked as soft deleted
                var entitiesToUpdate = entities.OfType<IOEntityDeletable>().ToList();
                if (entitiesToUpdate.Any())
                {
                    Context.Set<TEntity>().UpdateRange(entitiesToUpdate as TEntity);
                }

                await Context.SaveChangesAsync();
                return new IOResultMetadata(IOResultStatusEnum.Success, "Entities deleted successfully");
            }
            catch (Exception ex)
            {
                return new IOResultMetadata(IOResultStatusEnum.Error, ex.Message);
            }
        }

    }
}
