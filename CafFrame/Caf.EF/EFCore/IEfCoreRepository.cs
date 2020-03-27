using Caf.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.EF.EFCore
{
    public interface IEfCoreRepository<TEntity> : IEfCoreRepository<TEntity, long>, ICafRepository<TEntity> where TEntity : class
    {

    }

    public interface IEfCoreRepository<TEntity,Key> : ICafRepository<TEntity, Key> where TEntity : class
    {
        DbContext DbContext { get; }

        DbSet<TEntity> DbSet { get; }
    }
}
