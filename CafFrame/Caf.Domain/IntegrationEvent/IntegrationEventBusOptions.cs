using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.IntegrationEvent
{
    public class IntegrationEventBusOptions
    {
        public int ConsumerThreadCount { get; set; } = 1;
        public IntegrationEventBusOptions()
        {
            Extensions = new List<IOptionsExtension>();
        }
        internal IList<IOptionsExtension> Extensions { get; }
        public void RegisterExtension(IOptionsExtension extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension));
            }

            Extensions.Add(extension);
        }
    }
}
