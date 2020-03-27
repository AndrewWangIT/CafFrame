using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Auditing
{
    public interface IAuditingStore<TAuditingLogInfo> where TAuditingLogInfo : BaseAuditingLogInfo
    {
        void Save(TAuditingLogInfo auditInfo);

        //Task SaveAsync(TAuditingLogInfo auditInfo);
    }
}
