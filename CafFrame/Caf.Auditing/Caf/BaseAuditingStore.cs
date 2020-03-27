using Caf.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Auditing
{
    public abstract class BaseAuditingStore<TAuditingLogInfo> : IAuditingStore<TAuditingLogInfo> , ITransient where TAuditingLogInfo : BaseAuditingLogInfo
    {
        public abstract void Save(TAuditingLogInfo auditInfo);
        //public abstract Task SaveAsync(TAuditingLogInfo auditInfo);
    }
}
