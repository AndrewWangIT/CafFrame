using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.IntegrationEvent
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventTopicAttribute : Attribute
    {
        public string TopicName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicName"></param>
        public EventTopicAttribute(string topicName)
        {
            TopicName = topicName;
        }
    }
}
