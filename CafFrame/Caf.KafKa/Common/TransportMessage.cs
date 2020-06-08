using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Kafka.Common
{
    /// <summary>
    /// Message content field
    /// </summary>
    public class TransportMessage<T>
    {
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

        public T EventData { get { return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(Body)); } }

        public string GetId()
        {
            return Headers.TryGetValue(MessageHeaders.MessageId, out var value) ? value : null;
        }

        public string GetName()
        {
            return Headers.TryGetValue(MessageHeaders.MessageName, out var value) ? value : null;
        }

        public string GetGroup()
        {
            return Headers.TryGetValue(MessageHeaders.Group, out var value) ? value : null;
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
