using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.Module
{
    public interface IModuleContainer
    {
        IReadOnlyList<ICafModuleDescriptor> Modules { get; }
    }
}
