using Caf.Core.Module;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Caf.Core.DependencyInjection;
using System.Reflection;
using System.Linq;
using Caf.Core;

namespace Caf.Auditing
{
    public static class CafAuditingExtensions
    {
        public static void AddCafAuditing(this CafConfigurationContext context, Action<CafAuditingOptions> action)
        {
            CafAuditingOptions cafCorsOption = new CafAuditingOptions();
            action.Invoke(cafCorsOption);
            context.Services.AddObjectWrapper(cafCorsOption);
        }

        public static void AddAuditing(this CafConfigurationContext context,Assembly assembly)
        {
            var option = context.Services.GetSingletonInstance<IObjectWrapper<CafAuditingOptions>>().Value;
            foreach (var map in option.AttributeLogMappings)
            {
                context.Services.AddSingleton(map.LogType);
                var handleType = typeof(IAuditingLogHandle<,>).MakeGenericType(map.LogType,map.AttributeType);
                var handleImp = assembly.GetTypes().Where(o => o.IsClass && !o.IsAbstract && handleType.IsAssignableFrom(o)).FirstOrDefault();
                if(handleImp ==null)
                {
                    throw new CafException($"未实现AuditingInfo的Handle实现{handleType.ToString()}");
                }
                context.Services.AddTransient(handleType, handleImp);


                var storeType = typeof(IAuditingStore<>).MakeGenericType(map.LogType);
                var storeImp = assembly.GetTypes().Where(o => o.IsClass && !o.IsAbstract && storeType.IsAssignableFrom(o)).FirstOrDefault();
                if (storeImp == null)
                {
                    throw new CafException($"未实现AuditingInfo的Store实现{storeType.ToString()}");
                }
                context.Services.AddTransient(storeType, storeImp);
            }

        }
    }
}
