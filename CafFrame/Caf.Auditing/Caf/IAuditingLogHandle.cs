using AspectCore.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Auditing
{
    public interface IAuditingLogHandle<in TAuditingLogInfo, in TAuditingAttribute> where TAuditingLogInfo : BaseAuditingLogInfo where TAuditingAttribute : CafAuditedAttribute
    {
        Task PreAuditAsync(AspectContext context, TAuditingLogInfo auditingLogInfo, TAuditingAttribute attributeInfo);

        Task PostAuditAsync(AspectContext context, TAuditingLogInfo auditingLogInfo, TAuditingAttribute attributeInfo);
    }
}
