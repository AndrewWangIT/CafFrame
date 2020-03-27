using Caf.Core.DependencyInjection;
using Caf.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.Module
{
    public class CafApplicationContext: IServiceProviderAccessor
    {
        public IServiceProvider ServiceProvider { get; set; }
        public CafApplicationContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
