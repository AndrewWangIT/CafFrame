using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.IntegrationEvent
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventGroupAttribute : Attribute
    {
        public string GroupId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicName"></param>
        public EventGroupAttribute(string groupId)
        {
            GroupId = groupId;
        }
    }
}
