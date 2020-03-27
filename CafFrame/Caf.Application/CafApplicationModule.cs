using Caf.AutoMapper;
using Caf.Core.Module;
using Caf.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Application
{
    [UsingModule(typeof(CafAutoMapperModule))]
    [UsingModule(typeof(CafDomainModule))]
    public class CafApplicationModule:CafModule
    {
        public override void ConfigureServices(CafConfigurationContext context)
        {
            
        }
    }
}
