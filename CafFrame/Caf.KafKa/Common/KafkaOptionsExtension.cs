using Caf.Domain.IntegrationEvent;
using Caf.Kafka.Consumer;
using Caf.Kafka.Producer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Kafka.Common
{
    public sealed class CafKafkaOptionsExtension : IOptionsExtension
    {
        private readonly Action<KafkaOptions> _configure;

        public CafKafkaOptionsExtension(Action<KafkaOptions> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            services.Configure(_configure);

            services.AddSingleton<ITransport, KafkaTransport>();
            services.AddSingleton<IConsumerClientFactory, KafkaConsumerClientFactory>();
            services.AddSingleton<IConnectionPool, ConnectionPool>();
            services.AddSingleton<IIntegrationEventBus, KafkaIntegrationEventBus>();
        }
    }
    public static class KafkaOptionsExtensions
    {
        /// <summary>
        /// Configuration to use kafka in CAP.
        /// </summary>
        /// <param name="options">CAP configuration options</param>
        /// <param name="bootstrapServers">Kafka bootstrap server urls.</param>
        public static IntegrationEventBusOptions UseKafka(this IntegrationEventBusOptions options, string bootstrapServers)
        {
            return options.UseKafka(opt => { opt.Servers = bootstrapServers; });
        }
        /// <summary>
        /// Configuration to use kafka in CAP.
        /// </summary>
        /// <param name="options">CAP configuration options</param>
        /// <param name="configure">Provides programmatic configuration for the kafka .</param>
        /// <returns></returns>
        public static IntegrationEventBusOptions UseKafka(this IntegrationEventBusOptions options, Action<KafkaOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            options.RegisterExtension(new CafKafkaOptionsExtension(configure));

            return options;
        }
    }
}
