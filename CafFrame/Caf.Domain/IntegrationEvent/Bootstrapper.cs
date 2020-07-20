using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caf.Domain.IntegrationEvent
{
    /// <summary>
    /// Default implement of <see cref="T:DotNetCore.Caf.Internal.IBootstrapper" />.
    /// </summary>
    internal class Bootstrapper : BackgroundService
    {
        private readonly ILogger<Bootstrapper> _logger;

        public Bootstrapper(
            ILogger<Bootstrapper> logger,
            IEnumerable<IProcessingServer> processors)
        {
            _logger = logger;
            Processors = processors;
        }


        private IEnumerable<IProcessingServer> Processors { get; }

        public async Task BootstrapAsync(CancellationToken stoppingToken)
        {

            stoppingToken.Register(() =>
            {
                _logger.LogDebug("### background task is stopping.");

                foreach (var item in Processors)
                {
                    try
                    {
                        item.Dispose();
                    }
                    catch (OperationCanceledException ex)
                    {
                        _logger.LogError(ex,"");
                    }
                }
            });

            await BootstrapCoreAsync();

            _logger.LogInformation("### Caf started!");
        }

        protected virtual Task BootstrapCoreAsync()
        {
            foreach (var item in Processors)
            {
                try
                {
                    item.Start();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ProcessorsStartedError");
                }
            }

            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BootstrapAsync(stoppingToken);
        }
    }
}
