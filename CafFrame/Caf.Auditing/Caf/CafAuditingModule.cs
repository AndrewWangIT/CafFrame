
using Caf.Core.DynamicProxy;
using Caf.Core.Module;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Auditing
{
    public class CafAuditingModule: CafModule
    {
        public override void BeforeConfigureServices(CafConfigurationContext context)
        {
            context.Services.AddTransient<AuditingInterceptor>();
            context.Services.OnRegistred(AuditingInterceptorRegistrar.RegisterIfNeeded(context.Services));
        }
    }
}
