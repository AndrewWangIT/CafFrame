using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.IntegrationEvent
{
    public interface IConsumerClientFactory
    {
        /// <summary>
        /// Create a new instance of <see cref="IConsumerClient" />.
        /// </summary>
        /// <param name="groupId">message group number</param>
        IConsumerClient Create(string groupId);
    }
}
