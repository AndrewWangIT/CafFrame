using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Domain.Repository
{
    public interface IQueryRepository<TEntity, TKey> where TEntity : class
    {
        TEntity Get(TKey id);
        ValueTask<TEntity> GetAsync(TKey id);
        Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> where);
        TEntity GetOne(Expression<Func<TEntity, bool>> where);
        IQueryable<TEntity> QueryList(Expression<Func<TEntity, bool>> where);
        IQueryable<TEntity> GetAllNoTracking();
        IQueryable<T> FromSql<T>(string sql, params object[] parameters) where T : class;
        IQueryable<TEntity> QueryPagedList<TR>(int offset, int limit, Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TR>> orderby, bool isAsc, out int total);
        IQueryable<TEntity> QueryPagedList(int offset, int limit, Expression<Func<TEntity, bool>> where, List<Expression<Func<TEntity, object>>> orderbys, List<bool> isAscs, out int total);
        IQueryable<TEntity> QueryPagedList<TR>(int offset, int limit, IQueryable<TEntity> queryable, Expression<Func<TEntity, TR>> orderby, bool isAsc, out int total);


    }
}
