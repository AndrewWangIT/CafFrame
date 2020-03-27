using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Auditing
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class CafAuditedAttribute : Attribute
    {
        public CafAuditedAttribute()
        {

        }
    }
}
