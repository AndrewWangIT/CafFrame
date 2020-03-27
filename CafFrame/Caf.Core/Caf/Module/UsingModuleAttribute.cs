using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.Module
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=true)]
    public class UsingModuleAttribute : Attribute
    {
        public Type[] ModuleTypes { get; }
        public UsingModuleAttribute(params Type[] moduleTypes)
        {
            if (moduleTypes.IsNullOrEmpty())
            {
                throw new CafException($"moduleType不能为空");
            }                
            ModuleTypes = moduleTypes ?? new Type[0];
        }
        public virtual Type[] GetDependedTypes()
        {
            return ModuleTypes;
        }
    }
}
