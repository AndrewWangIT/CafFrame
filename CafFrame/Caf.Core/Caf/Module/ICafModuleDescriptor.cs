using Caf.Core.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Caf.Core.Module
{
    public interface ICafModuleDescriptor
    {
        public Type Type { get; }

        public Assembly Assembly { get; }

        public ICafModule Instance { get; }

        public bool IsLoadedAsPlugIn { get; }

        IReadOnlyList<ICafModuleDescriptor> Dependencies { get; }
    }
}