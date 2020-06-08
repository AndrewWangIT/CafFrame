using Caf.Domain.IntegrationEvent;
using Caf.Kafka.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Kafka.Producer
{
    public interface ITransport
    {
        BrokerAddress BrokerAddress { get; }

        Task<OperateResult> SendAsync(TransportMessage message);
    }
}
