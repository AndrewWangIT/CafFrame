using AspectCore.DynamicProxy;
using AspectCore.Extensions.Reflection;
using AspectCore.Injector;
using Caf.Core.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Auditing
{
    public class AuditingInterceptor : IInterceptor
    {
        public bool AllowMultiple { get; set; }

        public bool Inherited { get; set; }
        public int Order { get; set; }
        [FromContainer]
        public ILogger<AuditingInterceptor> _logger { get; set; }

        public async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var attributeType = context.ImplementationMethod.GetReflector().GetCustomAttributes(typeof(CafAuditedAttribute)).First();
            var cafAuditingOptions = context.ServiceProvider.Resolve<IObjectWrapper<CafAuditingOptions>>().Value;
            var logType = cafAuditingOptions.AttributeLogMappings.First(o => o.AttributeType == attributeType.GetType())?.LogType;
            
            if(logType==null)
            {
                await next(context);
                return;
            }
            var handleType = typeof(IAuditingLogHandle<,>).MakeGenericType(logType, attributeType.GetType());
            var handleImp = context.ServiceProvider.GetService(handleType);

            var storeType = typeof(IAuditingStore<>).MakeGenericType(logType);
            var storeImp = context.ServiceProvider.GetService(storeType);

            var logInfo = context.ServiceProvider.GetService(logType) as BaseAuditingLogInfo;
            var preAuditTask = handleType.GetMethod("PreAuditAsync").GetReflector().Invoke(handleImp, new object[] { context, logInfo, attributeType }) as Task;//(handle, context,logs);
            await preAuditTask;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogError("开始拦截");
                await next(context);
            }
            catch (Exception ex)
            {
                logInfo.Exceptions.Add(ex);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                logInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                logInfo.ExecutionTime = DateTime.Now;             
                var postAuditTask = handleType.GetMethod("PostAuditAsync").GetReflector().Invoke(handleImp, new object[] { context, logInfo, attributeType }) as Task;
                await postAuditTask;
                storeType.GetMethod("Save").GetReflector().Invoke(storeImp, new object[] { logInfo });
            }
        }
    }
}
