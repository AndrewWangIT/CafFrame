using Caf.Core.DependencyInjection;
using Caf.Domain.Entities;
using Caf.Domain.IntegrationEvent;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Kafka.Producer
{
    public class KafkaIntegrationEventBus : IIntegrationEventBus, ISingleton
    {
        private readonly ITransport _kafkaTransport;
        private readonly object _lock=new object();
        public KafkaIntegrationEventBus(ITransport kafkaTransport)
        {
            _kafkaTransport = kafkaTransport;
        }
        private ConcurrentDictionary<string, List<IntegrationHandlerDesc>> eventStore = new ConcurrentDictionary<string, List<IntegrationHandlerDesc>>();//eventTopic 与handle的映射关系
        public async Task publish<T>(T eventData) where T : IntegrationEvent
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add(KafkaHeaders.KafkaKey, eventData.Key);
            headers.Add(MessageHeaders.Topic, eventData.Topic);
            TransportMessage transportMessage = new TransportMessage(headers, JsonConvert.SerializeObject(eventData).GetBytes());
            var result = await _kafkaTransport.SendAsync(transportMessage);
            if (result.Succeeded)
            {
                //xxx
            }
            else
            {
                throw new EventPublishException(result.Exception);
            }
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var topicAttribute = typeof(T).GetCustomAttributes().OfType<EventTopicAttribute>().FirstOrDefault();
            var topicName = topicAttribute?.TopicName ?? this.GetType().FullName;
            var groupAttribute = typeof(TH).GetCustomAttributes().OfType<EventGroupAttribute>().FirstOrDefault();
            var groupId = groupAttribute?.GroupId ?? "default-group";
            lock(_lock)
            {
                var handler = new IntegrationHandlerDesc(typeof(TH), typeof(T), groupId, typeof(IIntegrationEventHandler<>).MakeGenericType(typeof(TransportMessage).MakeGenericType(typeof(T))).GetMethod("Handle"));
                if (eventStore.ContainsKey(topicName))
                {
                    var handlers = eventStore[topicName];
                    if(! handlers.Any(o=>o.GroupId==handler.GroupId && o.HandlerType==handler.HandlerType && o.IntegrationEventType==handler.IntegrationEventType))
                    {
                        handlers.Add(handler);
                    }
                }
                else
                {
                    eventStore.TryAdd(topicName, new List<IntegrationHandlerDesc> { handler });
                }
            }
        }


        public void Subscribe(Type eventDataType,Type eventHandlerType)
        {
            if(!typeof(IntegrationEvent).IsAssignableFrom(eventDataType) || !typeof(IIntegrationEventHandler<>).MakeGenericType(eventDataType).IsAssignableFrom(eventHandlerType))
            {
                throw new Exception("eventDataType参数不继承IntegrationEvent!");
            }
            var topicAttribute = eventDataType.GetCustomAttributes().OfType<EventTopicAttribute>().FirstOrDefault();
            var topicName = topicAttribute?.TopicName ?? this.GetType().FullName;
            var groupAttribute = eventHandlerType.GetCustomAttributes().OfType<EventGroupAttribute>().FirstOrDefault();
            var groupId = groupAttribute?.GroupId ?? "default-group";
            lock (_lock)
            {
                var handler = new IntegrationHandlerDesc(eventHandlerType, eventDataType, groupId, typeof(IIntegrationEventHandler<>).MakeGenericType(eventDataType).GetMethod("Handle"));
                if (eventStore.ContainsKey(topicName))
                {
                    var handlers = eventStore[topicName];
                    if (!handlers.Any(o => o.GroupId == handler.GroupId && o.HandlerType == handler.HandlerType && o.IntegrationEventType == handler.IntegrationEventType))
                    {
                        handlers.Add(handler);
                    }
                }
                else
                {
                    eventStore.TryAdd(topicName, new List<IntegrationHandlerDesc> { handler });
                }
            }
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var topicAttribute = this.GetType().GetCustomAttributes().OfType<EventTopicAttribute>().FirstOrDefault();
            var topicName = topicAttribute?.TopicName ?? this.GetType().FullName;
            List<IntegrationHandlerDesc> handlers = new List<IntegrationHandlerDesc>();
            if(eventStore.TryGetValue(topicName,out handlers))
            {
                foreach (var item in handlers.Where(o =>  o.HandlerType ==typeof(TH)))
                {
                    handlers.Remove(item);
                }           
            }
        }
        public List<IntegrationHandlerDesc> GetIntegrationHandlerDescs(string Topic)
        {
            return eventStore.ContainsKey(Topic) ? eventStore[Topic] : new List<IntegrationHandlerDesc>();
        }
        public Dictionary<string, List<string>> GetGroups()
        {
            Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();
            foreach (var item in eventStore)
            {
                foreach (var handler in item.Value)
                {
                    if(groups.ContainsKey(handler.GroupId))
                    {
                        if(!groups[handler.GroupId].Contains(item.Key))
                        {
                            groups[handler.GroupId].Add(item.Key);
                        }
                    }
                    else
                    {
                        groups.Add(handler.GroupId, new List<string> { item.Key });
                    }
                }
            }
            return groups;
        }
    }
}
