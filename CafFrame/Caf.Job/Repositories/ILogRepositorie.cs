using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caf.Job.Repositories
{
    public interface ILogRepositorie
    {
        Task<bool> RemoveErrLogAsync(string jobGroup, string jobName);
    }
}
