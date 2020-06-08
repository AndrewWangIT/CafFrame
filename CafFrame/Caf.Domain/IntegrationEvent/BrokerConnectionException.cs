using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.IntegrationEvent
{
    public class BrokerConnectionException : Exception
    {
        public BrokerConnectionException(Exception innerException)
            : base("Broker Unreachable", innerException)
        {

        }
    }
}
