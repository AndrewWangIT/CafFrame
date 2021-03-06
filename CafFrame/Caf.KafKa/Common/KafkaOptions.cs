using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caf.Kafka.Common
{
    public class KafkaOptions
    {
        /// <summary>
        /// librdkafka configuration parameters (refer to https://github.com/edenhill/librdkafka/blob/master/CONFIGURATION.md).
        /// <para>
        /// Topic configuration parameters are specified via the "default.topic.config" sub-dictionary config parameter.
        /// </para>
        /// </summary>
        public readonly ConcurrentDictionary<string, string> MainConfig;

        private IEnumerable<KeyValuePair<string, string>> _kafkaConfig;

        public KafkaOptions()
        {
            MainConfig = new ConcurrentDictionary<string, string>();
        }

        /// <summary>
        /// Producer connection pool size, default is 10
        /// </summary>
        public int ConnectionPoolSize { get; set; } = 10;

        /// <summary>
        /// The `bootstrap.servers` item config of <see cref="MainConfig" />.
        /// <para>
        /// Initial list of brokers as a CSV list of broker host or host:port.
        /// </para>
        /// </summary>
        public string Servers { get; set; }

        /// <summary>
        /// If you need to get offset and partition and so on.., you can use this function to write additional header into <see cref="CafHeader"/>
        /// </summary>
        public Func<ConsumeResult<string, byte[]>, List<KeyValuePair<string, string>>> CustomHeaders { get; set; }

        internal IEnumerable<KeyValuePair<string, string>> AsKafkaConfig()
        {
            if (_kafkaConfig == null)
            {
                if (string.IsNullOrWhiteSpace(Servers))
                {
                    throw new ArgumentNullException(nameof(Servers));
                }

                MainConfig["bootstrap.servers"] = Servers;
                MainConfig["queue.buffering.max.ms"] = "10";
                MainConfig["enable.auto.commit"] = "false";
                MainConfig["log.connection.close"] = "false";
                MainConfig["request.timeout.ms"] = "10000";
                MainConfig["message.timeout.ms"] = "10000";

                _kafkaConfig = MainConfig.AsEnumerable();
            }

            return _kafkaConfig;
        }
    }
}
