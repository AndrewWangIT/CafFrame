using AspectCore.DynamicProxy;
using Caf.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Auditing
{
    public abstract class BaseAuditingLogHandle<TAuditingLogInfo, TAuditingAttribute> : IAuditingLogHandle<TAuditingLogInfo, TAuditingAttribute>, ITransient where TAuditingLogInfo : BaseAuditingLogInfo where TAuditingAttribute : CafAuditedAttribute
    {
        public abstract Task PostAuditAsync(AspectContext context, TAuditingLogInfo auditingLogInfo, TAuditingAttribute attributeInfo);
        public abstract Task PreAuditAsync(AspectContext context, TAuditingLogInfo auditingLogInfo, TAuditingAttribute attributeInfo);
    }
}
