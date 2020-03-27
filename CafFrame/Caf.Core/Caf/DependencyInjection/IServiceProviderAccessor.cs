using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.DependencyInjection
{
    public interface IServiceProviderAccessor
    {
        IServiceProvider ServiceProvider { get; }
    }
}
