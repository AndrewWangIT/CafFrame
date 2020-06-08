using Caf.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caf.Domain.IntegrationEvent
{
    public abstract class AsyncIntegrationEventHandler<T> : IIntegrationEventHandler<T> ,ISingleton where T:IntegrationEvent
    {
        public abstract Task Handle(T evevtData, TransportMessage transportMessage, CancellationToken cancellationToken);

        public string Group
        {
            get
            {
                var groupAttribute = this.GetType().GetCustomAttributes().OfType<EventGroupAttribute>().FirstOrDefault();
                return groupAttribute?.GroupId ?? "default-group";
            }
        }
    }
}
