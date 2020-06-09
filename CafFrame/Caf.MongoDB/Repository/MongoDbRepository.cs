using AspectCore.Extensions.Reflection;
using Caf.Domain.Entities;
using Caf.MongoDB.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caf.MongoDB.Repository
{
    public class MongoDbRepository<TMongoDbContext, TEntity>
    : MongoDbRepository<TMongoDbContext, TEntity, ObjectId>,IMongoDbRepository<TMongoDbContext, TEntity>
    where TMongoDbContext : CafMongoDbContext
    where TEntity : class, IEntity<ObjectId>
    {
        public MongoDbRepository(IMongoDbContextProvider<TMongoDbContext> mongoDbContextProvider):base(mongoDbContextProvider)
        {

        }
    }

    public class MongoDbRepository<TMongoDbContext, TEntity, TKey>
    : IMongoDbRepository<TMongoDbContext, TEntity, TKey>
    where TMongoDbContext : CafMongoDbContext
    where TEntity : class, IEntity<TKey>
    {
        public bool IgnoreGlobalFilter { get; set; }
        public virtual TMongoDbContext DbContext => DbContextProvider.GetDbContext();
        protected IMongoDbContextProvider<TMongoDbContext> DbContextProvider { get; }
        public IMongoDatabase Database => DbContext.Database;
        public virtual IMongoCollection<TEntity> Collection => DbContext.Collection<TEntity>();

        public MongoDbRepository(IMongoDbContextProvider<TMongoDbContext> mongoDbContextProvider)
        {
            DbContextProvider = mongoDbContextProvider;
        }
        public TEntity Get(TKey id)
        {
            var entity = Find(id);

            return entity;
        }

        public async Task<TEntity> GetAsync(TKey id)
        {
            var entity = await FindAsync(id);
            return entity;
        }

        public virtual IMongoQueryable<TEntity> GetMongoQueryable()
        {
            return ApplyDataFilters(
                Collection.AsQueryable()
            );
        }
        public TEntity GetOne(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
        {
            return GetMongoQueryable().Where(where).FirstOrDefault();
        }

        public async Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
        {
            return await GetMongoQueryable().Where(where).FirstOrDefaultAsync();
        }

        public IQueryable<TEntity> QueryList(Expression<Func<TEntity, bool>> where)
        {
            return GetMongoQueryable().Where(where);
        }
        public virtual TEntity Find(TKey id)
        {
            return Collection.Find(CreateEntityFilter(id, true)).FirstOrDefault();
        }
        public virtual async Task<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await Collection
                .Find(CreateEntityFilter(id, true))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public long GetCount()
        {
            return GetMongoQueryable().LongCount();
        }

        public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await GetMongoQueryable().LongCountAsync(cancellationToken);
        }
        public void Delete(TEntity entity, bool autoSave = false)
        {

            if (entity is IHasSoftDelete softDeleteEntity)
            {
                softDeleteEntity.IsDeleted = true;
                var result = Collection.ReplaceOne(
                    CreateEntityFilter(entity.Id),
                    entity
                );
            }
            else
            {
                var result = Collection.DeleteOne(
                    CreateEntityFilter(entity.Id)
                );
            }
        }

        public async Task DeleteAsync(
            TEntity entity,
            CancellationToken cancellationToken = default)
        {
            if (entity is IHasSoftDelete softDeleteEntity)
            {
                softDeleteEntity.IsDeleted = true;
                var result = await Collection.ReplaceOneAsync(
                    CreateEntityFilter(entity.Id),
                    entity,
                    cancellationToken: cancellationToken
                );
            }
            else
            {
                var result = await Collection.DeleteOneAsync(
                    CreateEntityFilter(entity.Id),
                    cancellationToken
                );
            }
        }
        public void Delete(Expression<Func<TEntity, bool>> predicate, bool autoSave = false)
        {
            var entities = GetMongoQueryable()
                .Where(predicate)
                .ToList();

            foreach (var entity in entities)
            {
                Delete(entity, autoSave);
            }
        }

        public async Task DeleteAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var entities = await GetMongoQueryable()
                .Where(predicate)
                .ToListAsync(cancellationToken);

            foreach (var entity in entities)
            {
                await DeleteAsync(entity, cancellationToken);
            }
        }

        public async Task<TEntity> InsertAsync(
            TEntity entity,
            CancellationToken cancellationToken = default)
        {
            AutoSettingFieldForAdded(entity);
            await Collection.InsertOneAsync(
                entity,
                cancellationToken: cancellationToken
            );

            return entity;
        }

        protected virtual FilterDefinition<TEntity> CreateEntityFilter(TKey id, bool applyFilters = false)
        {
            var filters = new List<FilterDefinition<TEntity>>
            {
                Builders<TEntity>.Filter.Eq(e => e.Id, id)
            };

            if (applyFilters)
            {
                AddGlobalFilters(filters);
            }

            return Builders<TEntity>.Filter.And(filters);
        }
        protected virtual void AddGlobalFilters(List<FilterDefinition<TEntity>> filters)
        {
            if (IgnoreGlobalFilter)
            {
                return;
            }
            if (typeof(IHasSoftDelete).IsAssignableFrom(typeof(TEntity)))//IHasSoftDelete
            {
                filters.Add(Builders<TEntity>.Filter.Eq(e => ((IHasSoftDelete)e).IsDeleted, false));
            }
        }
        protected virtual TQueryable ApplyDataFilters<TQueryable>(TQueryable query)
    where TQueryable : IQueryable<TEntity>
        {
            if (!IgnoreGlobalFilter && typeof(IHasSoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                query = (TQueryable)query.Where(e => ((IHasSoftDelete)e).IsDeleted == false);
            }
            IgnoreGlobalFilter = false;
            return query;
        }


        private void AutoSettingFieldForAdded(TEntity entity)
        {
            if (entity is IHasCreationTime)
            {
                ((IHasCreationTime)entity).CreationTime = DateTime.Now;
            }
            //存在反射性能问题，后期优化
            var entityType = typeof(TEntity);
            if (entityType.GetInterfaces().Any(o => o.IsGenericType && (o.GetGenericTypeDefinition() == typeof(IHasCreator<>) || o.GetGenericTypeDefinition() == typeof(IHasCreatorWithReferenceTypeKey<>))))
            {
                if (entityType.GetProperties().First(o => o.Name == "CreatorId").GetReflector().GetValue(entity) == null)
                {
                    entityType.GetProperties().First(o => o.Name == "CreatorId").SetValue(entity, DbContext.CurrentCreatorId);
                }
            }
        }
    }

    public static class MongoDbRepositoryExt
    {
        public static MongoDbRepository<TMongoDbContext, TEntity, TKey> IgnoreGlobalFilter<TMongoDbContext, TEntity, TKey>(this MongoDbRepository<TMongoDbContext, TEntity, TKey> repository) where TMongoDbContext : CafMongoDbContext
        where TEntity : class, IEntity<TKey>
        {
            repository.IgnoreGlobalFilter = true;
            return repository;
        }
    }
}
