using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Domain.IntegrationEvent
{
    public interface IConsumerErrHandler
    {
        Task HandleConsumerErr(ConsumerMessageContext consumerMessageContext);
    }
    public class ConsumerMessageContext
    {
        public TransportMessage message { get; set; }
        public Exception ex { get; set; }
        public List<IntegrationHandlerDesc> integrationHandlerDescs { get; set; }
    }
}
