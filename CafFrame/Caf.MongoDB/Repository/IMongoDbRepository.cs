using Caf.Domain.Entities;
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
    public interface IMongoDbRepository<TMongoDbContext,TEntity> : IMongoDbRepository<TMongoDbContext,TEntity, ObjectId> where TEntity : class, IEntity<ObjectId>
    {


    }
    public interface IMongoDbRepository<TMongoDbContext,TEntity, TKey> 
    where TEntity : class, IEntity<TKey>
    {
        IMongoDatabase Database { get; }

        IMongoCollection<TEntity> Collection { get; }

        IMongoQueryable<TEntity> GetMongoQueryable();

        TEntity Get(TKey id);
        Task<TEntity> GetAsync(TKey id);
        Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default);
        TEntity GetOne(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default);
        IQueryable<TEntity> QueryList(Expression<Func<TEntity, bool>> where);

        Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
    }

}
