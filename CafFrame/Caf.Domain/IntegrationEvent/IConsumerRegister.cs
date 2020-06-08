using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.IntegrationEvent
{
    public interface IConsumerRegister : IProcessingServer
    {
        bool IsHealthy();

        void ReStart(bool force = false);
    }
}
