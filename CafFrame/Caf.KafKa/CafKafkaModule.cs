using Caf.Core.Module;
using Caf.Domain.IntegrationEvent;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Kafka
{
    public class CafKafkaModule : CafModule
    {
        public override void OnApplicationInitialization(CafApplicationContext context)
        {
            //var eventBus = context.ServiceProvider.GetRequiredService<IIntegrationEventBus>();
            
            //var a = context.ServiceProvider.GetServices(typeof(IIntegrationEventHandler));
            //foreach (var item in context.ServiceProvider.GetServices(typeof(IIntegrationEventHandler)))
            //{
            //    var da= item.GetType();
            //}
        }
    }
}
