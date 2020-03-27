using Caf.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;

namespace Caf.Core.Module
{
    public class CafModuleDescriptor:ICafModuleDescriptor
    {
        public Type Type { get; }

        public Assembly Assembly { get; }

        public ICafModule Instance { get; }

        public bool IsLoadedAsPlugIn { get; }

        public IReadOnlyList<ICafModuleDescriptor> Dependencies => _dependencies.ToImmutableList();
        private readonly List<ICafModuleDescriptor> _dependencies;

        public CafModuleDescriptor(
             Type type,
             ICafModule instance,
            bool isLoadedAsPlugIn)
        {
            Check.NotNull(type, nameof(type));
            Check.NotNull(instance, nameof(instance));

            if (!type.GetTypeInfo().IsAssignableFrom(instance.GetType()))
            {
                throw new ArgumentException($"Given module instance ({instance.GetType().AssemblyQualifiedName}) is not an instance of given module type: {type.AssemblyQualifiedName}");
            }

            Type = type;
            Assembly = type.Assembly;
            Instance = instance;
            IsLoadedAsPlugIn = isLoadedAsPlugIn;
            _dependencies = new List<ICafModuleDescriptor>();
        }

        public void AddDependency(ICafModuleDescriptor descriptor)
        {
            _dependencies.AddIfNotContains(descriptor);
        }

        public override string ToString()
        {
            return $"[CafModuleDescriptor {Type.FullName}]";
        }
    }
}
