using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caf.Job
{
    public class BaseResult
    {
        public int Code { get; set; } = 200;
        public string Msg { get; set; }
    }
}
