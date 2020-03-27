using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caf.EF.EFCore
{
    public class EFUOWConnection : IEFUOWConnection
    {
        public DbContext dbContext { get; set; }

        public IDbContextTransaction DbContextTransaction { get; set; }
        public void Commit()
        {
            DbContextTransaction.Commit();
        }

        public Task CommitAsync()
        {
            return DbContextTransaction.CommitAsync();
        }

        public void Dispose()
        {
            DbContextTransaction.Dispose();
        }

        public void Rollback()
        {
            DbContextTransaction.Rollback();
        }

        public Task RollbackAsync(CancellationToken cancellationToken)
        {
            return DbContextTransaction.RollbackAsync();
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return dbContext.SaveChangesAsync();
        }
    }
}
