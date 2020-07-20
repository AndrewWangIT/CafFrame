using Caf.Kafka.Common;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Caf.Domain.IntegrationEvent;

namespace Caf.Kafka.Consumer
{
    internal sealed class KafkaConsumerClient : IConsumerClient
    {
        private static readonly SemaphoreSlim ConnectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        private readonly string _groupId;
        private readonly KafkaOptions _kafkaOptions;
        private IConsumer<string, byte[]> _consumerClient;

        public KafkaConsumerClient(string groupId, IOptions<KafkaOptions> options)
        {
            _groupId = groupId;
            _kafkaOptions = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public event EventHandler<TransportMessage> OnMessageReceived;

        public event EventHandler<LogMessageEventArgs> OnLog;

        public BrokerAddress BrokerAddress => new BrokerAddress("Kafka", _kafkaOptions.Servers);

        public string CurrentGroupId => _groupId;

        public void Subscribe(IEnumerable<string> topics)
        {
            if (topics == null)
            {
                throw new ArgumentNullException(nameof(topics));
            }

            Connect();

            _consumerClient.Subscribe(topics);
        }

        public void Listening(TimeSpan timeout, CancellationToken cancellationToken)
        {
            Connect();

            while (true)
            {
                var consumerResult = _consumerClient.Consume(cancellationToken);

                if (consumerResult.IsPartitionEOF || consumerResult.Message.Value == null) continue;

                var headers = new Dictionary<string, string>(consumerResult.Message.Headers.Count);
                foreach (var header in consumerResult.Message.Headers)
                {
                    var val = header.GetValueBytes();
                    headers.Add(header.Key, val != null ? Encoding.UTF8.GetString(val) : null);
                }
                headers.Add(MessageHeaders.Group, _groupId);

                if (_kafkaOptions.CustomHeaders != null)
                {
                    var customHeaders = _kafkaOptions.CustomHeaders(consumerResult);
                    foreach (var customHeader in customHeaders)
                    {
                        headers.Add(customHeader.Key, customHeader.Value);
                    }
                }
                if(!headers.ContainsKey(MessageHeaders.Topic))
                {
                    headers.Add(MessageHeaders.Topic, consumerResult.Topic);
                }
                var message = new TransportMessage(headers, consumerResult.Message.Value);

                OnMessageReceived?.Invoke(consumerResult, message);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public void Commit(object sender)
        {
            _consumerClient.Commit((ConsumeResult<string, byte[]>)sender);
        }

        public void Reject(object sender)
        {
            _consumerClient.Assign(_consumerClient.Assignment);
        }

        public void Dispose()
        {
            _consumerClient?.Dispose();
        }

        public void Connect()
        {
            if (_consumerClient != null)
            {
                return;
            }

            ConnectionLock.Wait();

            try
            {
                if (_consumerClient == null)
                {
                    _kafkaOptions.MainConfig["group.id"] = _groupId;
                    _kafkaOptions.MainConfig["auto.offset.reset"] = "earliest";//初始化连接时读取最早的信息
                    var config = _kafkaOptions.AsKafkaConfig();

                    _consumerClient = new ConsumerBuilder<string, byte[]>(config)
                        .SetErrorHandler(ConsumerClient_OnConsumeError)
                        .Build();
                }
            }
            finally
            {
                ConnectionLock.Release();
            }
        }

        private void ConsumerClient_OnConsumeError(IConsumer<string, byte[]> consumer, Error e)
        {
            var logArgs = new LogMessageEventArgs
            {
                LogType = MqLogType.ServerConnError,
                Reason = $"An error occurred during connect kafka --> {e.Reason}"
            };
            OnLog?.Invoke(null, logArgs);
        }
    }
}
