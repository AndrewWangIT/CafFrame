using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.IntegrationEvent
{
    /// <summary>
    /// Message content field
    /// </summary>
    public class TransportMessage<T>: TransportMessage
    {
        public TransportMessage(IDictionary<string, string> headers, byte[] body):base(headers, body)
        {

        }
        public T EventData { get { return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(Body)); } }

    }
    public class TransportMessage
    {
        public TransportMessage()
        {

        }
        public TransportMessage(IDictionary<string, string> headers, byte[] body)
        {
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
            Body = body;
        }

        /// <summary>
        /// Gets the headers of this message
        /// </summary>
        public IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets the body object of this message
        /// </summary>
        public byte[] Body { get; }

        public string GetId()
        {
            return Headers.TryGetValue(MessageHeaders.MessageId, out var value) ? value : "";
        }
        public string GetTopic()
        {
            return Headers.TryGetValue(MessageHeaders.Topic, out var value) ? value : "";
        }
        public string GetName()
        {
            return Headers.TryGetValue(MessageHeaders.MessageName, out var value) ? value : "";
        }

        public string GetGroup()
        {
            return Headers.TryGetValue(MessageHeaders.Group, out var value) ? value : "";
        }
    }
    public class LogMessageEventArgs : EventArgs
    {
        public string Reason { get; set; }

        public MqLogType LogType { get; set; }
    }
    public enum MqLogType
    {
        //Kafka
        ConsumeError,
        ServerConnError,
    }
}
