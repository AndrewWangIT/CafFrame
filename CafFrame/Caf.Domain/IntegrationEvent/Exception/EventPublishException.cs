using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.IntegrationEvent
{
    public class EventPublishException : Exception
    {
        public EventPublishException(Exception innerException)
            : base("Event Publish Err", innerException)
        {

        }
    }
}
