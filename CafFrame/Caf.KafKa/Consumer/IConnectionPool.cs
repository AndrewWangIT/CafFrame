using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Caf.Kafka.Consumer
{
    public interface IConnectionPool
    {
        string ServersAddress { get; }

        IProducer<string, byte[]> RentProducer();

        bool Return(IProducer<string, byte[]> producer);
    }
}
