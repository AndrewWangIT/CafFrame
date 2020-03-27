using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.UnitOfWork.Interface
{
    public interface IUowHelper
    {
        bool IsExistUow { get; }
        IUnitOfWork Create();
        IUOWConnection GetOrAddUOWConnection(string key, IUOWConnection uOWConnection);
        bool IsExistConnectionKey(string key);
    }
}
