using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.IntegrationEvent
{
    public interface IProcessingServer : IDisposable
    {
        void Pulse();

        void Start();
    }
}
