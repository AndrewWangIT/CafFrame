using Caf.AutoMapper;
using Caf.Core.Module;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain
{
    [UsingModule(typeof(CafAutoMapperModule))]
    public class CafDomainModule:CafModule
    {
    }
}
