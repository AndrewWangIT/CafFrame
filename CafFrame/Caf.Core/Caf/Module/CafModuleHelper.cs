using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Caf.Core.Module
{
    public static class CafModuleHelper
    {
        /// <summary>
        /// 加载所有的系统应用moudle
        /// </summary>
        /// <returns></returns>
        public static List<Type> FindAllModuleTyps(Type startupModuleType)
        {
            var moudelType = new List<Type>();
            AddModuleAndDependenciesResursively(moudelType, startupModuleType);
            return moudelType;
        }
        /// <summary>
        /// 获取Module依赖
        /// </summary>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public static List<Type> GetDependedModuleTypes(Type moduleType)
        {
            var dependencies=new List<Type>();
            CafModule.CheckCafModuleType(moduleType);
            var aa = moduleType.GetCustomAttributes();
            var dependencyDescriptors = moduleType.GetCustomAttributes(typeof(UsingModuleAttribute))
                .ToList();
            foreach (var item in dependencyDescriptors)
            {
                foreach (var type in ((UsingModuleAttribute)item).GetDependedTypes())
                {
                    dependencies.AddIfNotContains(type);
                }
            }
            return dependencies;
        }
        private static void AddModuleAndDependenciesResursively(List<Type> moudelsTypes,Type moduleType)
        {
            CafModule.CheckCafModuleType(moduleType);
            if(moudelsTypes.Contains(moduleType))
            {
                return;
            }
            moudelsTypes.Add(moduleType);
            foreach (var type in GetDependedModuleTypes(moduleType))
            {
                AddModuleAndDependenciesResursively(moudelsTypes, type);
            }
        }
    }
}
