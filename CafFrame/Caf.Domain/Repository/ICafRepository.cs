using Caf.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Caf.Domain.Repository
{
    public interface ICafRepository<TEntity, TKey> : IQueryRepository<TEntity, TKey> where TEntity:class
    {   
        TEntity Insert(TEntity entity, bool autoSave = false);
        Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false);
        Task DeleteAsync(TKey key);
        void Delete(TKey key);
        void Delete(TEntity entity);
        void Delete(Expression<Func<TEntity, bool>> where);
        TEntity Update(TEntity entity, bool autoSave = false);
        Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false);
        Task BulkInsertAsync<T>(IList<T> entities) where T : class;
        void SaveChanges();
        Task SaveChangesAsync();
        int ExecuteSqlRaw(string sql, params object[] parameters);
        Task<int> ExecuteSqlRawAsync(string sql);
        Task<bool> BulkInsertAsync(DataTable data, int? batchSize);
        Task BulkDeleteAsync<T>(IList<T> entities) where T : class;
    }
    public interface ICafRepository<TEntity>: ICafRepository<TEntity,long> where TEntity : class
    {

    }

}
