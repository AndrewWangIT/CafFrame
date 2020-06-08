using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Domain.IntegrationEvent
{
    public interface IIntegrationEventBus
    {
        Task publish<T>(T eventData) where T: IntegrationEvent;
        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
        void Subscribe(Type eventDataType, Type eventHandlerType);
        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;
        List<IntegrationHandlerDesc> GetIntegrationHandlerDescs(string Topic);
        Dictionary<string, List<string>> GetGroups();

    }
}
