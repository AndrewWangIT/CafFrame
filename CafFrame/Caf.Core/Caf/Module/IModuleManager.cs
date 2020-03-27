using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.Module
{
    public interface IModuleManager
    {
        void InitializeModules(CafApplicationContext context);

        void ShutdownModules(CafApplicationContext context);
    }
}
