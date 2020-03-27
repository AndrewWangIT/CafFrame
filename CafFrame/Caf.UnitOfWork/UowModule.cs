using Caf.Core.DynamicProxy;
using Caf.Core.Module;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.UnitOfWork
{
    public class UowModule:CafModule
    {
        public override void BeforeConfigureServices(CafConfigurationContext context)
        {
            context.Services.OnRegistred(UowRegistar.RegisterIfNeeded(context.Services));
        }
    }
}
