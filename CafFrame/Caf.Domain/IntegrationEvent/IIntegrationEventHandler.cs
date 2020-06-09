using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caf.Domain.IntegrationEvent
{
    public interface IIntegrationEventHandler
    {
    }
    public interface IIntegrationEventHandler<T>: IIntegrationEventHandler where T: IntegrationEvent
    {
        Task Handle(T evevtData, TransportMessage transportMessage, CancellationToken cancellationToken);
        string Group { get; }
    }
}
