using Caf.Core.DependencyInjection;
using Caf.Domain.IntegrationEvent;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CafApi.Event
{

    [EventTopic("Topic1")]
    public class MyIntegrationData: IntegrationEvent
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
    [EventGroup("group1")]
    public class MyIntegrationHandler: AsyncIntegrationEventHandler<MyIntegrationData>
    {
        private readonly ILogger<MyIntegrationHandler> _logger;
        public MyIntegrationHandler(ILogger<MyIntegrationHandler> logger)
        {
            _logger = logger;
        }
        public override Task Handle(MyIntegrationData evevtData, TransportMessage transportMessage, CancellationToken cancellationToken)
        {
            var a = evevtData;
            _logger.LogInformation(evevtData.Name);
            return Task.CompletedTask;
        }
    }

    [EventGroup("group2")]
    public class MySecondIntegrationHandler : AsyncIntegrationEventHandler<MyIntegrationData>
    {
        private readonly ILogger<MyIntegrationHandler> _logger;
        public MySecondIntegrationHandler(ILogger<MyIntegrationHandler> logger)
        {
            _logger = logger;
        }
        public override Task Handle(MyIntegrationData evevtData, TransportMessage transportMessage, CancellationToken cancellationToken)
        {
            var a = evevtData;
            _logger.LogInformation(evevtData.Age.ToString());
            return Task.CompletedTask;
        }
    }
}
