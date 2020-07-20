using Caf.Domain.IntegrationEvent;
using Caf.Kafka.Common;
using Caf.Kafka.Consumer;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caf.Kafka.Producer
{
    public class KafkaTransport : ITransport
    {
        private readonly IConnectionPool _connectionPool;
        private readonly ILogger _logger;

        public KafkaTransport(ILogger<KafkaTransport> logger, IConnectionPool connectionPool)
        {
            _logger = logger;
            _connectionPool = connectionPool;
        }

        public BrokerAddress BrokerAddress => new BrokerAddress("Kafka", _connectionPool.ServersAddress);

        public async Task<OperateResult> SendAsync(TransportMessage message)
        {
            var producer = _connectionPool.RentProducer();

            try
            {
                var headers = new Confluent.Kafka.Headers();

                foreach (var header in message.Headers)
                {
                    headers.Add(header.Value != null
                        ? new Header(header.Key, Encoding.UTF8.GetBytes(header.Value))
                        : new Header(header.Key, null));
                }
                CancellationToken a = new CancellationToken();
                var result = await producer.ProduceAsync(message.GetTopic(), new Message<string, byte[]>
                {
                    Headers = headers,
                    Key = message.Headers.TryGetValue(KafkaHeaders.KafkaKey, out string kafkaMessageKey) && !string.IsNullOrEmpty(kafkaMessageKey) ? kafkaMessageKey : message.GetId(),
                    Value = message.Body
                },a );

                if (result.Status == PersistenceStatus.Persisted || result.Status == PersistenceStatus.PossiblyPersisted)
                {
                    _logger.LogDebug($"kafka topic message [{message.GetName()}] has been published.");

                    return OperateResult.Success;
                }

                throw new Exception("kafka message persisted failed!");
            }
            catch (Exception ex)
            {
                var wapperEx = new Exception(ex.Message, ex);

                return OperateResult.Failed(wapperEx);
            }
            finally
            {
                var returned = _connectionPool.Return(producer);
                if (!returned)
                {
                    producer.Dispose();
                }
            }
        }
    }
}
