using Caf.Core.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Domain.IntegrationEvent
{
    public class DefaultConsumerErrHandler : IConsumerErrHandler,ISingleton
    {
        private readonly ILogger<DefaultConsumerErrHandler> _logger;
        public DefaultConsumerErrHandler(ILogger<DefaultConsumerErrHandler> logger)
        {
            _logger = logger;
        }
        public Task HandleConsumerErr(ConsumerMessageContext consumerMessageContext)
        {
            _logger.LogError(consumerMessageContext.ex, "消费出错");
            return Task.CompletedTask;
        }
    }
}
