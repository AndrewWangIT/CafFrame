using Caf.MongoDB.MongoDB;
using Caf.UnitOfWork.Interface;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caf.MongoDB.Uow
{
    public class MongoDbUOWConnection: IUOWConnection, ITransactionConnection
    {
        public ICafMongoDbContext DbContext { get; set; }

        public IClientSession ClientSession { get; set; }

        public void Commit()
        {
            ClientSession.CommitTransaction();
        }

        public Task CommitAsync()
        {
            return ClientSession.CommitTransactionAsync();
        }

        public void Dispose()
        {
            ClientSession.Dispose();
        }

        public void Rollback()
        {
            ClientSession.AbortTransaction();
        }

        public Task RollbackAsync(CancellationToken cancellationToken)
        {
            return ClientSession.AbortTransactionAsync();
        }
    }
}
