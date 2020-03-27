using Caf.UnitOfWork.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.EF.EFCore
{
    public interface IEFUOWConnection: IUOWConnection, ITransactionConnection, ICanSaveChanges
    {
    }
}
