using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caf.UnitOfWork.Interface
{
    public interface ITransactionConnection: IDisposable
    {
        void Commit();

        Task CommitAsync();

        void Rollback();

        Task RollbackAsync(CancellationToken cancellationToken);
    }
}
