using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Caf.EF.EFCore
{
    public class CafEfCoreRepository<TEntity, Key> : IEfCoreRepository<TEntity, Key> where TEntity : class
    {
        private readonly IDbContextProvider<TEntity> _dbContextProvider;
        public CafEfCoreRepository(IDbContextProvider<TEntity> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }
        public virtual DbContext DbContext => _dbContextProvider.GetDbContext();

        public DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

        public void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }
        public void Delete(Key key)
        {
            var entity = DbSet.Find(key);
            if(entity !=null)
            {
                DbSet.Remove(entity);
            }           
        }
        public Task DeleteAsync(Key key)
        {
            var entity = DbSet.Find(key);
            if (entity != null)
            {
                DbSet.Remove(entity);            
            }
            return Task.CompletedTask;
        }
        public void Delete(Expression<Func<TEntity, bool>> where)
        {
            foreach (var entity in GetQueryable().Where(where).ToList())
            {
                Delete(entity);
            }
        }

        public IQueryable<T> FromSql<T>(string sql ,params object[] parameters) where T:class
        {
            return DbContext.Set<T>().FromSqlRaw(sql, parameters);
        }

        public int ExecuteSqlRaw(string sql, params object[] parameters)
        {
            return DbContext.Database.ExecuteSqlRaw(sql, parameters);
        }

        public TEntity Get(Key id)
        {
            return DbSet.Find(id);
        }

        public IQueryable<TEntity> GetAllNoTracking()
        {
            return GetQueryable().AsNoTracking();
        }

        public ValueTask<TEntity> GetAsync(Key id)
        {
            return DbSet.FindAsync(id);
        }

        public TEntity GetOne(Expression<Func<TEntity, bool>> where)
        {
            return GetQueryable().FirstOrDefault(where);
        }

        public Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> where)
        {
            var entity = GetQueryable().FirstOrDefault(where);
            return Task.FromResult(entity);
        }

        public TEntity Insert(TEntity entity, bool isSave = false)
        {
            DbSet.Add(entity);
            if(isSave)
            {
                DbContext.SaveChanges();
            }
            return entity;
        }

        public async Task<TEntity> InsertAsync(TEntity entity,bool isSave = false)
        {
            DbSet.Add(entity);
            if (isSave)
            {
                await DbContext.SaveChangesAsync();
            }
            return entity;
        }

        public IQueryable<TEntity> QueryList(Expression<Func<TEntity, bool>> where)
        {
            return GetQueryable().Where(where);
        }

        public IQueryable<TEntity> QueryPagedList<TR>(int offset, int limit, Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TR>> orderby, bool isAsc, out int total)
        {
            offset = offset >= 0 ? offset : 0;
            limit = limit >= 0 ? limit : 10;

            total = DbSet.Count(where);
            if (isAsc)
            {
                return GetQueryable().Where(where).OrderBy(orderby).Skip(offset).Take(limit);
            }
            return GetQueryable().Where(where).OrderByDescending(orderby).Skip(offset).Take(limit);
        }

        public IQueryable<TEntity> QueryPagedList<TR>(int offset, int limit, IQueryable<TEntity> queryable, Expression<Func<TEntity, TR>> orderby, bool isAsc, out int total)
        {
            offset = offset >= 0 ? offset : 0;
            limit = limit >= 0 ? limit : 10;

            total = queryable.Count();
            if (isAsc)
            {
                return queryable.OrderBy(orderby).Skip(offset).Take(limit);
            }
            return queryable.OrderByDescending(orderby).Skip(offset).Take(limit);
        }


        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await DbContext.SaveChangesAsync();
        }

        public TEntity Update(TEntity entity, bool autoSave = false)
        {
            DbContext.Attach(entity);

            var updatedEntity = DbContext.Update(entity).Entity;

            if (autoSave)
            {
                DbContext.SaveChanges();
            }

            return updatedEntity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false)
        {
            DbContext.Attach(entity);

            var updatedEntity = DbContext.Update(entity).Entity;

            if (autoSave)
            {
                await DbContext.SaveChangesAsync();
            }

            return updatedEntity;
        }

        protected IQueryable<TEntity> GetQueryable()
        {
            return DbSet.AsQueryable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="data"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public async Task<bool> BulkInsertAsync(DataTable data, int? batchSize)
        {
            string tableName = DbContext.Model.FindEntityType(typeof(TEntity)).GetTableName();
            var conn = DbContext.Database.GetDbConnection() as SqlConnection;
            using (SqlBulkCopy copy = new SqlBulkCopy(conn))
            {
                if (batchSize > 0)
                {
                    copy.BatchSize = batchSize.Value;
                }

                foreach (DataColumn item in data.Columns)
                {
                    copy.ColumnMappings.Add(item.ColumnName, item.ColumnName);
                }

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                copy.DestinationTableName = tableName;
                await copy.WriteToServerAsync(data);
                return true;
            }
        }

        public Task BulkInsertAsync<T>(IList<T> entities) where T : class
        {
            return DbContext.BulkInsertAsync(entities);
        }
        public Task BulkDeleteAsync<T>(IList<T> entities) where T : class
        {
            return DbContext.BulkDeleteAsync(entities);
        }
        public IQueryable<TEntity> QueryPagedList(int offset, int limit, Expression<Func<TEntity, bool>> where, List<Expression<Func<TEntity, object>>> orderbys, List<bool> isAscs, out int total)
        {
            offset = offset >= 0 ? offset : 0;
            limit = limit >= 0 ? limit : 10;

            total = DbSet.Count(where);
            var query = GetQueryable().Where(where);
            for (int i = 0; i < orderbys.Count; i++)
            {
                if (i > 0)
                {
                    if (isAscs[i])
                    {
                        query = ((IOrderedQueryable<TEntity>) query).ThenBy(orderbys[i]);
                    }
                    query = ((IOrderedQueryable<TEntity>)query).ThenByDescending(orderbys[i]);
                }
                else
                {
                    if (isAscs[i])
                    {
                        query = query.OrderBy(orderbys[i]);
                    }
                    query = query.OrderByDescending(orderbys[i]);
                }
                
            }
            return query.Skip(offset).Take(limit);
        }

        public async Task<int> ExecuteSqlRawAsync(string sql)
        {
            return await DbContext.Database.ExecuteSqlRawAsync(sql);
        }
    }
    public class CafEfCoreRepository<TEntity>: CafEfCoreRepository<TEntity,long>, IEfCoreRepository<TEntity> where TEntity : class
    {
        public CafEfCoreRepository(IDbContextProvider<TEntity> dbContextProvider):base(dbContextProvider)
        {
        }
    }
}
