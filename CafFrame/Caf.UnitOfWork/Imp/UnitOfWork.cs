using Caf.Core.DependencyInjection;
using Caf.UnitOfWork.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.UnitOfWork.Imp
{
    public class UnitOfWork : IUnitOfWork, IScoped
    {
        public ConnectionDic ConnectionDic { get; set; } = new ConnectionDic();

        public void Commit()
        {
            try
            {
                SaveChanges();
                CommitTransactions();
            }
            catch(Exception ex)
            {
                Rollback();
                throw ex;
            }
        }
        public void Dispose()
        {
            DisposeTransactions();
        }

        public void Rollback()
        {
            foreach (var connection in ConnectionDic)
            {
                if(connection.Value is ITransactionConnection)
                {
                    ((ITransactionConnection)connection.Value).Rollback();
                }
            }
        }
        public void SaveChanges()
        {
            foreach (var connection in ConnectionDic)
            {
                if (connection.Value is ICanSaveChanges)
                {
                    ((ICanSaveChanges)connection.Value).SaveChanges();
                }
            }
        }
        private void CommitTransactions()
        {
            foreach (var connection in ConnectionDic)
            {
                if (connection.Value is ITransactionConnection)
                {
                    ((ITransactionConnection)connection.Value).Commit();
                }
            }
        }
        private void DisposeTransactions()
        {
            foreach (var connection in ConnectionDic)
            {
                if (connection.Value is ITransactionConnection)
                {
                    ((ITransactionConnection)connection.Value).Dispose();
                }
            }
        }

    }
}
