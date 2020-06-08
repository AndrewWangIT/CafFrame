using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Caf.Domain.IntegrationEvent
{
    public class IntegrationEvent
    {
        public string Topic
        {
            get
            {
                var topicAttribute = this.GetType().GetCustomAttributes().OfType<EventTopicAttribute>().FirstOrDefault();
                return topicAttribute?.TopicName ?? this.GetType().FullName;
            }        
        }
        public string Key { get; set; }
    }
}
