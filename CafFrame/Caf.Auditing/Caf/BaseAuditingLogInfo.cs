using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Auditing
{
    public abstract class BaseAuditingLogInfo
    {
        public List<Exception> Exceptions { get; } = new List<Exception>();
        public int ExecutionDuration { get; set; }
        public DateTime ExecutionTime { get; set; }
    }
}
